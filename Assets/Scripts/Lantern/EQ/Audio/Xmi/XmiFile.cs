using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Tools;

namespace Lantern.EQ.Audio.Xmi
{
    public class XmiFile
    {
        //--Fields
        private Chunk[] chks;
        private CatChunk catChk;
        private FormChunk[] xmidChks;

        //--Properties
        public CatChunk Cat
        {
            get { return catChk; }
        }
        public FormChunk[] XmidiTracks
        {
            get { return xmidChks; }
        }
        public Chunk[] Chunks
        {
            get { return chks; }
        }

        //--Methods
        public XmiFile(Chunk[] chunks)
        {
            chks = chunks;
            catChk = FindChunk<CatChunk>();
            if (catChk != null)
            {
                xmidChks = catChk.SubChunks.OfType<FormChunk>().Where(fc => fc.TypeId == "XMID").ToArray();
            }
        }
        public T FindChunk<T>(int startIndex = 0) where T : Chunk
        {
            for (int x = startIndex; x < chks.Length; x++)
            {
                if (chks[x] is T)
                    return (T)chks[x];
            }
            return default(T);
        }

        public Stream WriteMidiTrack(int trackNumber)
        {
            if (trackNumber >= XmidiTracks.Length || XmidiTracks[trackNumber] == null)
            {
                throw new ArgumentException("Invalid track number.", nameof(trackNumber));
            }

            var stream = new MemoryStream();
            var xmidiTrack = XmidiTracks[trackNumber];
            var events = TranslateXmi(xmidiTrack, out var ppqn)
                    .OrderBy(e => e.Time).ToList();
            var timeDivision = new TicksPerQuarterNoteTimeDivision((short)ppqn);

            // xmi2mid will do a MultiSequence (type 2) if the original file had multiple tracks
            // which meltysynth will complain about
            using (var tokensWriter = MidiFile.WriteLazy(stream, settings: null, MidiFileFormat.SingleTrack, timeDivision))
            using (var objectsWriter = new TimedObjectsWriter(tokensWriter))
            {
                tokensWriter.StartTrackChunk();
                objectsWriter.WriteObjects(events);
            }

            stream.Position = 0;

            return stream;
        }

        public Stream WriteMidiTrackSequence(int trackNumber, int sequenceNumber, ChannelFlag channelMask = ChannelFlag.Unset)
        {
            var stream = WriteMidiTrack(trackNumber);

            var xmidiTrack = XmidiTracks[trackNumber];
            var branchLocations = xmidiTrack.BranchLocations;

            if (sequenceNumber == -1 || sequenceNumber >= branchLocations.Count || branchLocations.Count == 0)
            {
                return stream;
            }

            var midiFile = MidiFile.Read(stream);
            var markerEvents = midiFile.GetTimedEvents().Where(e => e.Event is MarkerEvent).ToList();

            var startTime = new MidiTimeSpan();
            var endTime = new MidiTimeSpan(markerEvents[sequenceNumber].Time);

            if (sequenceNumber > 0)
            {
                startTime = new MidiTimeSpan(markerEvents[sequenceNumber - 1].Time);
            }

            var partLength = endTime.Subtract(startTime, TimeSpanMode.TimeTime);

            var midiSequence = midiFile.TakePart(startTime, partLength);

            // support for filtering out channels
            // used primarily for character select loop section.
            if (channelMask != ChannelFlag.Unset)
            {
                // there shouldn't be more than one track chunk.
                foreach (var trackChunk in midiSequence.GetTrackChunks())
                {
                    using (var notesManager = trackChunk.ManageNotes())
                    {
                        notesManager.Objects.RemoveAll(note => {
                            var bitflagChannel = (ChannelFlag)(1 << (int)note.Channel);
                            return (channelMask & bitflagChannel) == 0;
                        });
                    }
                }
            }

            stream = new MemoryStream();
            midiSequence.Write(stream);
            stream.Position = 0;

            return stream;
        }

        ICollection<ITimedObject> TranslateXmi(FormChunk xmidiTrack, out long ppqn)
        {
            var result = new List<ITimedObject>();
            byte? channelEventStatusByte = null;
            ppqn = 0;

            long time = 0;
            long tempo = SetTempoEvent.DefaultMicrosecondsPerQuarterNote;
            var tempoSet = false;

            var eventChunk = xmidiTrack.SubChunks.OfType<EventChunk>().FirstOrDefault();
            if (eventChunk == null)
            {
                return result;
            }

            var xmiData = eventChunk.Data;
            var stream = new MemoryStream(xmiData);
            var reader = new MidiReader(stream, new ReaderSettings());

            var dwm = typeof(MidiEvent).Assembly;
            var eventReaderInterface = dwm.GetType("Melanchall.DryWetMidi.Core.IEventReader");
            var eventReaderType = dwm.GetType("Melanchall.DryWetMidi.Core.EventReaderFactory");
            var readerInfo = eventReaderType.GetMethod("GetReader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var readMethodInfo = eventReaderInterface.GetMethod("Read");

            while (reader.Position < reader.Length)
            {
                MidiEvent midiEvent;
                TimedEvent timedEvent;

                if (xmidiTrack.BranchLocations.TryGetValue(reader.Position, out var branchIndex))
                {
                    midiEvent = new MarkerEvent($":XBRN:{branchIndex:X2}");
                    timedEvent = new TimedEvent(midiEvent, time);
                    result.Add(timedEvent);
                }

                long deltaTime = reader.ReadXmiVlqLongNumber();
                if (deltaTime < 0)
                    deltaTime = 0;

                var statusByte = reader.ReadByte();
                if (statusByte <= SevenBitNumber.MaxValue)
                {
                    if (channelEventStatusByte == null)
                    {
                        // TODO: log warning
                        // throw new UnexpectedRunningStatusException();
                        continue;
                    }

                    statusByte = channelEventStatusByte.Value;
                    reader.Position--;
                }

                // midiEvent = ReadEvent(reader, statusByte);
                var eventReader = readerInfo.Invoke(null, new object[] { statusByte, true });
                midiEvent = (MidiEvent)readMethodInfo.Invoke(eventReader, new object[] { reader, new ReadingSettings(), statusByte });

                if (midiEvent == null)
                {
                    // TODO: log warning
                    continue;
                }

                if (midiEvent is EndOfTrackEvent)
                {
                    break;
                }

                if (midiEvent is ChannelEvent)
                {
                    channelEventStatusByte = statusByte;
                }

                midiEvent.DeltaTime = deltaTime * 3;
                time += midiEvent.DeltaTime;

                // needs to happen after the time is advanced.
                if (midiEvent is SetTempoEvent tempoEvent)
                {
                    // Skip any other tempo changes (but still advance the time)
                    if (tempoSet)
                    {
                        continue;
                    }

                    tempo = tempoEvent.MicrosecondsPerQuarterNote;
                    tempoSet = true;
                }

                timedEvent = new TimedEvent(midiEvent, time);
                result.Add(timedEvent);

                // Add note off event
                if (midiEvent is NoteOnEvent noteOnEvent)
                {
                    var durationInIntervals = reader.ReadVlqLongNumber();

                    // xmi2mid appears to always use note off velocity 0, not the note on velocity.
                    var noteOffEvent = new NoteOffEvent(noteOnEvent.NoteNumber, velocity: (SevenBitNumber)0)
                    {
                        Channel = noteOnEvent.Channel
                    };
                    timedEvent = new TimedEvent(noteOffEvent, time + (durationInIntervals * 3));
                    result.Add(timedEvent);
                }
            }

            var ppqnFloat = tempo * 3f / 25000f * 3f;
            ppqn = Convert.ToInt32(ppqnFloat);

            return result;
        }

        // Replaced with reflection.
        // private MidiEvent ReadEvent(MidiReader midiReader, byte statusByte)
        // {
        //     var settings = new ReadingSettings();

        //     var eventReader = EventReaderFactory.GetReader(statusByte, smfOnly: true);
        //     return eventReader.Read(midiReader, settings, statusByte);
        // }
    }
}
