using Lantern.EQ.Audio;
using Lantern.EQ.Sound;
using UnityEngine;

namespace Lantern.EQ.Characters
{
    public class CharacterSoundLogic : MonoBehaviour
    {
        private CharacterSoundData _sounds;
        private AudioSource _audioSource;
        private AudioSource _audioSourceLoop;
        private AudioSource _audioSourceWalkRun;

        // TODO: Maybe it's best if for the player, we just disable this script
        private bool _isPlayer;

        public void FindAudioClips(ICharacterSoundLocator locator, IAudioSourceProcessor audioSourceProcessor, int raceId, int genderId, int skin, bool isPlayer)
        {
            _sounds = locator.GetCharacterSounds(raceId, genderId, skin);
            CreateAudioSources(isPlayer, audioSourceProcessor);
        }

        private void CreateAudioSources(bool isPlayer, IAudioSourceProcessor audioSourceProcessor)
        {
            if (_sounds == null)
            {
                return;
            }

            _isPlayer = isPlayer;
            _audioSource = AudioHelper.AddCharacterAudioSource(gameObject, 6);
            _audioSource.spatialBlend = _isPlayer ? 0f : 1f;
            audioSourceProcessor?.Process(_audioSource);

            if (_sounds.Sounds.ContainsKey(CharacterSoundType.Loop))
            {
                _audioSourceLoop = AudioHelper.AddCharacterAudioSource(gameObject, 6);
                _audioSourceLoop.spatialBlend = _isPlayer ? 0f : 1f;
                _audioSourceLoop.loop = true;
                audioSourceProcessor?.Process(_audioSourceLoop);
                PlaySound(CharacterSoundType.Loop);
            }

            bool hasWalkRunSounds = _sounds.Sounds.ContainsKey(CharacterSoundType.Walking) ||
                                    _sounds.Sounds.ContainsKey(CharacterSoundType.Running);
            if (hasWalkRunSounds)// or is an npc type))
            {
                _audioSourceWalkRun = AudioHelper.AddCharacterAudioSource(gameObject, 6);
                _audioSourceWalkRun.spatialBlend = _isPlayer ? 0f : 1f;
                _audioSourceWalkRun.loop = true;
                audioSourceProcessor?.Process(_audioSourceWalkRun);
            }
        }

        private AudioSource GetAudioSourceForType(CharacterSoundType type)
        {
            if (type == CharacterSoundType.Loop)
            {
                return _audioSourceLoop;
            }

            if (AudioHelper.IsWalkOrRunSound(type))
            {
                return _audioSourceWalkRun;
            }

            return _audioSource;
        }

        public void InterruptWalkRunSound()
        {
            if (_audioSourceWalkRun != null)
            {
                _audioSourceWalkRun.clip = null;
                _audioSourceWalkRun.Stop();
            }
        }

        public void PlaySound(CharacterSoundType type)
        {
            if (_sounds == null)
            {
                return;
            }

            if (_isPlayer && AudioHelper.IsSilentPlayerSound(type))
            {
                return;
            }

            if (type == CharacterSoundType.Death)
            {
                if (_audioSourceLoop != null)
                {
                    _audioSourceLoop.Stop();
                    _audioSourceLoop.clip = null;
                }

                if (_audioSourceWalkRun != null)
                {
                    _audioSourceWalkRun.Stop();
                    _audioSourceWalkRun.clip = null;
                }
            }

            if (!_sounds.Sounds.TryGetValue(type, out var clips))
            {
                return;
            }

            if (clips.Count == 0)
            {
                return;
            }

            var clip = clips[Random.Range(0, clips.Count)];
            var source = GetAudioSourceForType(type);

            if (source == null)
            {
                Debug.LogError($"Invalid audio source for type: {type}");
                return;
            }

            if (source.isPlaying)
            {
                source.Stop();
            }

            if (AudioHelper.IsWalkOrRunSound(type))
            {
                if (_isPlayer)
                {
                    return;
                }

                _audioSourceWalkRun.clip = clip;
                _audioSourceWalkRun.Play();
            }

            source.clip = clip;
            source.Play();
        }
    }
}
