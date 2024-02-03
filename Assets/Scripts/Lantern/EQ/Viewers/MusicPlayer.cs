using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure.EQ.MeltySynth;
using Lantern.EQ.AssetBundles;
using Lantern.EQ.Audio;
using TMPro;
using UnityEngine;

namespace Scenes.EQ.MusicPlayer
{
    public class MusicPlayer : MonoBehaviour
    {
        public string SoundFontFile = "synthusr_samplefix.sf2";
        public string FileName;
        public bool Loop;
        public bool ReverbChorus;
        public float MasterVolume = 0.28f;
        public int MaximumPolyphony = 64;
        public int TrackNumber;
        public ChannelFlag ChannelMask;

        public bool IsPlaying => !_sequencer?.EndOfSequence ?? false;

        [SerializeField]
        private TextMeshProUGUI _fileNameLabel;
        [SerializeField]
        private TextMeshProUGUI _trackLabel;
        [SerializeField]
        private TextMeshProUGUI _loopLabel;
        [SerializeField]
        private TextMeshProUGUI _sfLabel;
        private AssetBundle _musicAssetBundle;
        [SerializeField]
        private List<string> _midiFiles;
        private int _midiFileIndex;

        private MidiTrackCollection _midiFile;
        private MidiFile _currentPlaybackMidi;
        private Synthesizer _synthesizer;
        private MidiFileSequencer _sequencer;
        private SoundFont _soundFont;

        private int _seqBranchZeroIndex;
        private bool _playPending;

        void Awake()
        {
            Application.targetFrameRate = 60;

            if (!LoadMusicBundle())
            {
                Debug.LogError("Failed to load Music_Midi bundle");
                return;
            }

            _midiFiles = _musicAssetBundle.GetAllAssetNames()
                .Select(filepath => Path.GetFileName(filepath))
                .ToList();

            SelectFile(0);
            ChangeLooping(true);
            CreateSynth();
            _sfLabel.SetText(SoundFontFile);
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                ChangeFile(-1);
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                ChangeFile(1);
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                ChangeTrack(1);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                ChangeTrack(-1);
            }

            if (Input.GetKeyUp(KeyCode.L))
            {
                ChangeLooping(!Loop);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (IsPlaying)
                {
                    Stop();
                }
                else
                {
                    Play();
                }
            }
        }

        private bool LoadMusicBundle()
        {
            const LanternAssetBundleId bundleId = LanternAssetBundleId.Music_Midi;
            var bundleVersion = AssetBundleVersions.GetVersion(bundleId);

            if (bundleVersion == null)
            {
                return false;
            }

            var bundleName = bundleId.ToString().ToLower() + "-" + bundleVersion.ToString().Replace('.', '_');
            var bundleFilepath = Path.Combine(Application.streamingAssetsPath, "AssetBundles", bundleName);
            _musicAssetBundle = AssetBundle.LoadFromFile(bundleFilepath);

            return _musicAssetBundle != null;
        }

        public MidiTrackCollection LoadMidiTrackCollection(string fileName)
        {
            return _musicAssetBundle.LoadAsset<MidiTrackCollection>(fileName);
        }

        private MidiFile LoadPlaybackMidiFile()
        {
            var midiTrack = _midiFile.MidiTracks.ElementAtOrDefault(TrackNumber);
            if (midiTrack == null)
            {
                return null;
            }

            using var stream = new MemoryStream(midiTrack.Bytes);
            return new MidiFile(stream, MidiFileLoopType.FinalFantasy);
        }

        public void Play()
        {
            _playPending = true;
        }

        private void PlayMidi()
        {
            if (_currentPlaybackMidi == null)
            {
                return;
            }

            _playPending = false;
            _sequencer.Play(_currentPlaybackMidi, Loop);
        }

        public void Stop()
        {
            _sequencer.Stop();
        }

        private void SelectFile(int index)
        {
            FileName = _midiFiles[index];
            _midiFile = LoadMidiTrackCollection(FileName);

            SelectTrack(0);

            _fileNameLabel.SetText($"File: {FileName}");

            if (IsPlaying)
            {
                Play();
            }
        }

        private void SelectTrack(int index)
        {
            var trackCount = _midiFile.MidiTracks.Count;
            TrackNumber = Mathf.Clamp(index, 0, trackCount);
            _currentPlaybackMidi = LoadPlaybackMidiFile();

            _trackLabel.SetText($"Track: {TrackNumber + 1}/{trackCount}");

            if (IsPlaying)
            {
                Play();
            }
        }

        private void ChangeFile(int change)
        {
            _midiFileIndex = (_midiFiles.Count + _midiFileIndex + change) % _midiFiles.Count;
            SelectFile(_midiFileIndex);
        }

        private void ChangeTrack(int change)
        {
            var trackIndex = (_midiFile.MidiTracks.Count + TrackNumber + change) % _midiFile.MidiTracks.Count;
            SelectTrack(trackIndex);
        }

        private void ChangeLooping(bool looping)
        {
            Loop = looping;
            _loopLabel.SetText("Loop: " + (Loop ? "Yes" : "No"));
        }

        private void CreateSynth()
        {
            var sfFilePath = Path.Combine(Application.streamingAssetsPath, "Soundfont", SoundFontFile);
            _soundFont = new SoundFont(sfFilePath);

            var settings = new SynthesizerSettings(AudioSettings.outputSampleRate)
            {
                MaximumPolyphony = MaximumPolyphony,
                EnableReverbAndChorus = ReverbChorus,
            };

            _synthesizer = new Synthesizer(_soundFont, settings)
            {
                MasterVolume = MasterVolume,
            };
            _sequencer = new MidiFileSequencer(_synthesizer);

            _sequencer.OnSendMessage += HandleOnMidiMessage;
        }

        private void HandleOnMidiMessage(Synthesizer synthesizer, int channel, int command, int data1, int data2)
        {
            // Debug.Log($"MidiMessageCallback: {channel}, {command}, {data1}, {data2}");

            switch (command)
            {
                case 0x90: // NoteOn
                    var bitflagChannel = (ChannelFlag)(1 << channel);
                    if (ChannelMask != ChannelFlag.Unset && (ChannelMask & bitflagChannel) == 0)
                    {
                        return;
                    }
                    break;
                case 0xb0: // CC
                    // SEQ_INDEX
                    if (data1 == 120 && data2 == 0)
                    {
                        _seqBranchZeroIndex = _sequencer.MessageIndex;
                    }

                    // CALLBACK_PFX
                    if (data1 == 108 && channel == 0 && data2 == 0)
                    {
                        _sequencer.Seek(_seqBranchZeroIndex);
                        return;
                    }
                    break;
            }

            synthesizer.ProcessMidiMessage(channel, command, data1, data2);
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (_playPending)
            {
                PlayMidi();
                return;
            }

            if (!IsPlaying)
            {
                return;
            }

            _sequencer.RenderInterleaved(data);
        }
    }
}
