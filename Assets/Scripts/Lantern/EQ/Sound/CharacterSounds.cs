using System;
using System.Collections.Generic;
using System.Linq;
using Infastructure.SerializableDictionary;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(CharacterAudioClips))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{
}
#endif

public enum CharacterSoundType
{
    Idle,
    GetHit,
    Jump,
    Loop,
    Gasp,
    Death,
    Drown,
    Walking,
    Running,
    Attack,
    SAttack,
    TAttack,
    Passive,
    Sit,
    Crouch,
    Treading,
    Swim,
    Kneel,
    Kick,
    Pierce,
    TwoHandSlash,
    TwoHandBlunt,
    Archery,
    FlyingKick,
    RapidPunch,
    LargePunch,
    Bash
}

[Serializable]
public class CharacterAudioClips : SerializableDictionary<CharacterSoundType, AudioClipGroup>
{
}

[Serializable]
public class VariantAudioClip
{
    public int VariantId;
    public CharacterAudioClips Clips;
}

[Serializable]
public class AudioClipGroup
{
    public List<AudioClip> Clips;
}

public class CharacterSounds : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceLoop;

    [SerializeField] private AudioSource _audioSourceOneShot;

    [SerializeField] List<VariantAudioClip> _variantAudio;

    [SerializeField] CharacterAudioClips _currentClip;
    
    private bool _hasAudio = false;

    private void Awake()
    {
        if (_audioSourceOneShot != null)
        {
            _audioSourceOneShot.volume = 0.3f;
            _audioSourceOneShot.spatialBlend = 1f;
            _audioSourceOneShot.rolloffMode = AudioRolloffMode.Linear;
            _audioSourceOneShot.minDistance = 0f;
            _audioSourceOneShot.maxDistance = 30f;
        }
        
        if (_audioSourceLoop != null)
        {
            _audioSourceLoop.volume = 0.3f;
            _audioSourceLoop.spatialBlend = 1f;
            _audioSourceLoop.rolloffMode = AudioRolloffMode.Linear;
            _audioSourceLoop.minDistance = 0f;
            _audioSourceLoop.maxDistance = 30f;
        }

        SetCurrentVariant(0);
    }
    
    public void AnimationPlayed(string animationName)
    {
        if (!_hasAudio)
        {
            return;
        }
        
        if (_currentClip == null)
        {
            return;
        }

        CharacterSoundType soundType = CharacterSoundType.Passive;

        switch (animationName)
        {
            case "c01":
                soundType = CharacterSoundType.Kick;
                break;
            case "c02":
                soundType = CharacterSoundType.Pierce;
                break;
            case "c03":
                soundType = CharacterSoundType.TwoHandSlash;
                break;
            case "c04":
                soundType = CharacterSoundType.TwoHandBlunt;
                break;
            case "c07":
                soundType = CharacterSoundType.Bash;
                break;
            case "c09":
                soundType = CharacterSoundType.Archery;
                break;
            case "c05":
            case "c06":
            case "c08":
            case "c10":
            case "c11":
                soundType = CharacterSoundType.Attack;
                break;
            case "d01":
                soundType = CharacterSoundType.GetHit;
                break;
            case "d02":
                soundType = CharacterSoundType.GetHit;
                break;
            case "d03":
                soundType = CharacterSoundType.GetHit;
                break;
            case "d04":
                soundType = CharacterSoundType.Drown;
                break;
            case "d05":
                soundType = CharacterSoundType.Death;
                break;
            case "l01":
                //soundType = CharacterSoundType.Walking;
                return;
                break;
            case "l02":
                //soundType = CharacterSoundType.Running;
                return;
                break;
            case "l03":
            case "l04":
                soundType = CharacterSoundType.Jump;
                break;
            case "l06":
                soundType = CharacterSoundType.Crouch;
                break;
            case "l09":
                soundType = CharacterSoundType.Treading;
                break;
            case "p01":
                soundType = CharacterSoundType.Loop;
                break;
            case "p02":
                soundType = CharacterSoundType.Sit;
                break;
            case "p05":
                soundType = CharacterSoundType.Kneel;
                break;
            case "p06":
                soundType = CharacterSoundType.Swim;
                break;
            case "o01":
                soundType = CharacterSoundType.Idle;
                break;
            case "t04":
            case "t05":
            case "t06":
                soundType = CharacterSoundType.TAttack;
                break;
            case "t07":
                soundType = CharacterSoundType.FlyingKick;
                break;
            case "t08":
                soundType = CharacterSoundType.RapidPunch;
                break;
            case "t09":
                soundType = CharacterSoundType.LargePunch;
                break;
        }

        if (!_currentClip.ContainsKey(soundType))
        {
            if (_audioSourceOneShot != null && _audioSourceOneShot.isPlaying && _audioSourceOneShot.loop)
            {
                _audioSourceOneShot.Stop();
                _audioSourceOneShot.clip = null;
            }

            return;
        }

        // Handling for loop sounds already playing
        if (soundType == CharacterSoundType.Loop)
        {
            HandleLoopingSounds();
            return;
        }

        if (soundType == CharacterSoundType.Death)
        {
            if (_audioSourceLoop != null)
            {
                _audioSourceLoop.Stop();
                _audioSourceLoop.clip = null;
            }
        }

        var clips = _currentClip[soundType];

        AudioClip clip = clips.Clips[UnityEngine.Random.Range(0, clips.Clips.Count)];

        bool isLoopedSound = clip.name.EndsWith("loop") || clip.name.EndsWith("lp");

        if (isLoopedSound && _audioSourceOneShot.isPlaying && _audioSourceOneShot.clip == clip)
        {
            return;
        }
        
        _audioSourceOneShot.clip = clip;
        _audioSourceOneShot.Play();
        _audioSourceOneShot.loop = isLoopedSound;
    }

    public void AddSoundClip(CharacterSoundType type, AudioClip clip, int variant)
    {
        if (type == CharacterSoundType.Loop)
        {
            if (_audioSourceLoop == null)
            {
                _audioSourceLoop = gameObject.AddComponent<AudioSource>();
                _audioSourceLoop.loop = true;
                _audioSourceLoop.playOnAwake = false;
                _audioSourceLoop.dopplerLevel = 0f;
            }
        }
        else
        {
            if (_audioSourceOneShot == null)
            {
                _audioSourceOneShot = gameObject.AddComponent<AudioSource>();
                _audioSourceOneShot.loop = false;
                _audioSourceOneShot.playOnAwake = false;
                _audioSourceOneShot.dopplerLevel = 0f;
            }
        }

        if (_variantAudio == null)
        {
            _variantAudio = new List<VariantAudioClip>();
        }

        VariantAudioClip variantClips = null;

        foreach (var clipList in _variantAudio)
        {
            if (clipList.VariantId == variant)
            {
                variantClips = clipList;
                break;
            }
        }

        // Add new variant if it doesn't yet exist
        if (variantClips == null)
        {
            variantClips = new VariantAudioClip {Clips = new CharacterAudioClips(), VariantId = variant};
            _variantAudio.Add(variantClips);
        }

        CharacterAudioClips characterClips = variantClips.Clips;

        if (!characterClips.ContainsKey(type))
        {
            characterClips[type] = new AudioClipGroup
            {
                Clips = new List<AudioClip>()
            };
        }

        characterClips[type].Clips.Add(clip);
    }

    public void SetCurrentVariant(int variantId)
    {
        if (_variantAudio == null || _variantAudio.Count == 0)
        {
            return;
        }

        if (_variantAudio.Count == 1)
        {
            _currentClip = _variantAudio[0].Clips;
            HandleLoopingSounds();
            return;
        }

        CharacterAudioClips newClips = null;

        foreach (var variant in _variantAudio)
        {
            if (variant.VariantId == variantId)
            {
                newClips = variant.Clips;
                break;
            }
        }

        // Fallback
        if (newClips == null)
        {
            newClips = _variantAudio[0].Clips;
        }

        _currentClip = newClips;
        
        HandleLoopingSounds();
    }

    private void HandleLoopingSounds()
    {
        if (_audioSourceLoop == null)
        {
            return;
        }
        
        if (_currentClip.ContainsKey(CharacterSoundType.Loop))
        {
            if (_audioSourceLoop.clip == null)
            {
                _audioSourceLoop.clip = _currentClip[CharacterSoundType.Loop].Clips.First();
                _audioSourceLoop.Play();
            }
        }
        else
        {
            if (_audioSourceLoop.clip != null)
            {
                _audioSourceLoop.clip = null;
                _audioSourceLoop.Stop();
            }
        }
    }
}