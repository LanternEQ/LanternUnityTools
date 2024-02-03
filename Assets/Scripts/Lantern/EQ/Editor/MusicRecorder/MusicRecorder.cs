using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Infrastructure.EQ.MeltySynth;
using Infrastructure.EQ.TextParser;
using Infrastructure.Lantern;
using Lantern.EQ.AssetBundles;
using Lantern.EQ.Audio;
using Lantern.EQ.Audio.Xmi;
using Lantern.EQ.Data;
using Lantern.EQ.Editor.Helpers;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using UnityEditor;
using UnityEngine;
using DryWetMidiFile = Melanchall.DryWetMidi.Core.MidiFile;
using MidiFile = Infrastructure.EQ.MeltySynth.MidiFile;

namespace Lantern.EQ.Editor.MusicRecorder
{
    public class MusicRecorder : LanternEditorWindow
    {
        private static int _channels = 2;
        private static int _sampleRate = 48000;
        private static float _masterVolume = 0.28f;
        private static string _soundFontName = "synthusr_samplefix.sf2";

        private static bool recordOnlyRequiredTracks = true;

        private static List<string> text1 = new()
        {
            "This process will convert EverQuest XMI files into WAV audio recordings. When recording for the LanternEQ client, you only need a subset of all tracks as there are duplicates.",
            "The recording process can take 20+ minutes. Recording all audio will take much longer."
        };

        private static List<string> text2 = new()
        {
            "EverQuest XMI files must be located in:",
            "\fAssets/EQAssets/music/",
        };

        private static List<string> text3 = new()
        {
            "Recorded audio will be output to:",
            "\fAssets/Content/AssetBundleContent/Music_Audio/"
        };

        [MenuItem("EQ/Assets/Record Music", false, 21)]
        public static void ShowWindow()
        {
            GetWindow<MusicRecorder>("Record Music", typeof(EditorWindow));
        }

        private void OnGUI()
        {
            DrawInfoBox(text1, "d_console.infoicon");
            DrawInfoBox(text2, "d_Collab.FolderConflict");
            DrawInfoBox(text3, "d_Collab.FolderMoved");

            DrawHorizontalLine();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Record only required tracks");
            recordOnlyRequiredTracks = EditorGUILayout.Toggle(recordOnlyRequiredTracks);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Start Recording"))
            {
                CreateAllAudioTracks();
            }
        }

        private static void GetLoopPoints(string name, XmiFile xmiFile, int trackIndex, StringBuilder writer)
        {
            if (xmiFile == null)
            {
                return;
            }
            var midiStream = xmiFile.WriteMidiTrack(trackIndex);
            var midiFile = DryWetMidiFile.Read(midiStream);
            var loopStart = GetLoopStartTimeSeconds(midiFile);
            var loopEnd = GetLoopEndTimeSeconds(midiFile);
            if (loopStart > 0f || loopEnd > 0f)
            {
                writer.AppendLine($"{name},{loopStart},{loopEnd}");
            }
        }

        private static void CreateAllAudioTracks()
        {
            var usedTracksPath = Path.Combine(Application.dataPath, "Content/ClientData/music_tracks_used.txt");
            var usedTracksText = File.ReadAllText(usedTracksPath);

            if (string.IsNullOrEmpty(usedTracksText))
            {
                return;
            }

            var usedTracks = TextParser.ParseTextByNewline(usedTracksText);

            var tracksPath = Path.Combine(Application.streamingAssetsPath, "ClientData/Music/music_tracks.txt");
            var tracksText = File.ReadAllText(tracksPath);

            if (string.IsNullOrEmpty(tracksText))
            {
                return;
            }

            var trackDictionary = TextParser.ParseTextToDictionaryOfStringList(tracksText);
            var createdAudio = new List<string>();

            var loopPointsSb = new StringBuilder();

            var startTime = EditorApplication.timeSinceStartup;

            foreach (var xmi in trackDictionary)
            {
                for (var i = 0; i < xmi.Value.Count; i++)
                {
                    var name = xmi.Value[i];
                    if (!usedTracks.Contains(name) || createdAudio.Contains(name))
                    {
                        continue;
                    }

                    var xmiFile = LoadXmi(xmi.Key);
                    RecordAudio(CreateSequencer(),xmiFile , i, -1, name, ChannelFlag.Unset);
                    GetLoopPoints(name, xmiFile, i, loopPointsSb);
                    createdAudio.Add(name);
                }
            }

            var openerXmi = LoadXmi("opener4");
            CreateCharacterSelectMusic(openerXmi);
            //GetLoopPoints("opener4", openerXmi, loopPointsSb);

            if (loopPointsSb.Length != 0)
            {
                var folderPath = Path.Combine(PathHelper.GetAssetBundleContentPath(), "Music_Audio");
                var filePath = Path.Combine(folderPath, "loop_points.txt");
                File.WriteAllText(filePath, loopPointsSb.ToString());
            }

            EditorUtility.DisplayDialog("MusicRecorder",
                $"Music recording finished in {(int) (EditorApplication.timeSinceStartup - startTime)} seconds", "OK");
        }

        private static XmiFile LoadXmi(string name)
        {
            var filePath = Path.Combine(PathHelper.GetEqAssetPath(), "Music", $"{name}.xmi");
            var xmiFs = File.OpenRead(filePath);
            var xmiReader = new XmiFileReader(xmiFs);
            var xmiFile = xmiReader.ReadXmiFile();
            return xmiFile;
        }

        private static void CreateCharacterSelectMusic(XmiFile openerXmi)
        {
            RecordAudio(CreateSequencer(), openerXmi, 1, 0, "character_select-intro", ChannelFlag.Unset);
            RecordAudio(CreateSequencer(), openerXmi, 1, 2, "character_select-outro", ChannelFlag.Unset);

            var races = Enum.GetNames(typeof(PlayerRaceId));

            foreach (var race in races)
            {
                if (!Enum.TryParse<ChannelFlag>(race, out var channelMask))
                {
                    Debug.LogError($"Failed to find channel mask for {race}");
                    return;
                }


                RecordAudio(CreateSequencer(), openerXmi, 1, 1, "character_select-" + race.ToLower(), channelMask);
            }
        }

        private static void RecordAudio(MidiFileSequencer sequencer, XmiFile xmiFile, int trackIndex, int sequenceIndex, string trackName, ChannelFlag channel)
        {
            var midiStream = xmiFile.WriteMidiTrackSequence(trackIndex, sequenceIndex, channel);
            var midiFile = new MidiFile(midiStream);
            sequencer.Play(midiFile, false);

            // The output buffer.
            var lenSamples = (int)(_sampleRate * midiFile.Length.TotalSeconds / sequencer.Speed);
            if (lenSamples == 0)
            {
                Debug.LogError("Track {trackName} has no length and cannot be created.");
                return;
            }

            var buffer = new float[lenSamples * _channels];

            // Render the waveform.
            sequencer.RenderInterleaved(buffer);

            Debug.Log($"Creating clip: {trackName}");
            var clip = AudioClip.Create(trackName, lenSamples, _channels, _sampleRate, false);
            clip.SetData(buffer, 0);

            var folderPath = Path.Combine(PathHelper.GetAssetBundleContentPath(), "Music_Audio");
            var wavPath = Path.Combine(folderPath, trackName + ".wav");
            SavWav.Save(wavPath, clip);
            AssetDatabase.ImportAsset(wavPath);

            AudioImporter importer = AssetImporter.GetAtPath(wavPath) as AudioImporter;
            if (importer != null)
            {
                importer.defaultSampleSettings = new AudioImporterSampleSettings
                {
                    loadType = AudioClipLoadType.Streaming,
                    compressionFormat = AudioCompressionFormat.Vorbis,
                    quality = 0.7f
                };
                importer.loadInBackground = !trackName.Contains("character_select");
                importer.preloadAudioData = false;
                AssetDatabase.ImportAsset(wavPath);
                AssetDatabase.Refresh();
            }

            // This could be moved out
            ImportHelper.TagAllAssetsForBundles(folderPath, LanternAssetBundleId.Music_Audio.ToString().ToLower());
        }

        private static MidiFileSequencer CreateSequencer()
        {
            var sfFilePath = Path.Combine(Application.streamingAssetsPath, "Soundfont", _soundFontName);
            var settings = new SynthesizerSettings(_sampleRate)
            {
                MaximumPolyphony = 256,
                EnableReverbAndChorus = true,
            };

            var synthesizer = new Synthesizer(sfFilePath, settings)
            {
                MasterVolume = _masterVolume,
            };
            return new MidiFileSequencer(synthesizer);
        }

        private static double GetLoopStartTimeSeconds(DryWetMidiFile midiFile)
        {
            var tempoMap = midiFile.GetTempoMap();
            var timedEvents = midiFile.GetTimedEvents();

            var timedEvent = timedEvents.FirstOrDefault(e =>
            {
                if (e.Event is not ControlChangeEvent ccEvent)
                {
                    return false;
                }

                // Loop start as SEQ_INDEX 0
                if (ccEvent.ControlNumber == 120 && ccEvent.ControlValue == 0)
                {
                    return true;
                }

                // Xmi Loop Start Message
                if (ccEvent.ControlNumber == 116)
                {
                    return true;
                }

                return false;
            });

            return timedEvent?.TimeAs<MetricTimeSpan>(tempoMap)?.TotalSeconds ?? 0;
        }

        private static double GetLoopEndTimeSeconds(DryWetMidiFile midiFile)
        {
            var tempoMap = midiFile.GetTempoMap();
            var timedEvents = midiFile.GetTimedEvents();

            var timedEvent = timedEvents.FirstOrDefault(e =>
            {
                if (e.Event is not ControlChangeEvent ccEvent)
                {
                    return false;
                }

                // Loop end as CALLBACK_PFX
                if (ccEvent.ControlNumber == 108 && ccEvent.Channel == 0 && ccEvent.ControlValue == 0)
                {
                    return true;
                }

                // Xmi Loop End Message
                if (ccEvent.ControlNumber == 117)
                {
                    return true;
                }

                return false;
            });

            return timedEvent?.TimeAs<MetricTimeSpan>(tempoMap)?.TotalSeconds ?? 0;
        }
    }
}
