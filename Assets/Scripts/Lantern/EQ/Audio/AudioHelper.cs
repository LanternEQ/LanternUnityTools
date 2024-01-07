using Lantern.EQ.Data;
using Lantern.EQ.Lantern;
using Lantern.EQ.Sound;
using UnityEngine;

namespace Lantern.EQ.Audio
{
    public static class AudioHelper
    {
        public static AudioSource AddCharacterAudioSource(GameObject gameObject, int size)
        {
            var audioSource = Create3dAudioSource(gameObject);
            audioSource.volume = EqConstants.AudioVolumeCharacter;
            audioSource.maxDistance = GetNpcAudioDistance(size);
            var curve = GetFalloff(EqConstants.AudioVolumeCharacter);
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
            return audioSource;
        }

        public static AudioSource AddDoorAudioSource(GameObject gameObject)
        {
            var audioSource = Create3dAudioSource(gameObject);
            audioSource.volume = EqConstants.AudioVolumeCharacter;
            audioSource.maxDistance = 50f;
            var curve = GetFalloff(EqConstants.AudioVolumeDoor);
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
            return audioSource;
        }

        public static AudioSource AddTemporaryAudioSource(GameObject gameObject)
        {
            var audioSource = Create3dAudioSource(gameObject);
            audioSource.volume = 0.75f; // guess
            audioSource.maxDistance = 100f; // guess
            var curve = GetFalloff(EqConstants.AudioVolumeCharacter);
            audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
            return audioSource;
        }

        public static AnimationCurve GetSound3DFalloff(int multiplier)
        {
            // Yes, this is correct
            if (multiplier < 3)
            {
                multiplier = 1;
            }

            var curve = GetFalloff(EqConstants.AudioVolumeSound3d, multiplier);
            return curve;
        }

        private static float GetNpcAudioDistance(int size)
        {
            int units = 0;
            switch (size)
            {
                case 1:
                    units = 48;
                    break;
                case 2:
                    units = 95;
                    break;
                case 3:
                    units = 143;
                    break;
                case 4:
                    units = 192;
                    break;
                case 5:
                    units = 240;
                    break;
                case 6:
                    units = 287;
                    break;
                case 7:
                    units = 167;
                    break;
                case 8:
                    units = 192;
                    break;
                case 9:
                    units = 215;
                    break;
                case 10:
                    units = 240;
                    break;
            }

            return units * LanternConstants.WorldScale;
        }

        private static AnimationCurve GetFalloff(float maxVolume, int multiplier = 1)
        {
            var ac = new AnimationCurve();
            AddCurveKey(ref ac, 0f, 1f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.03472222222f, 1f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.04166666667f, 1f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.04305555556f, 0.9730769231f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.04444444444f, 0.9384615385f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.04583333333f, 0.9184615385f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.04722222222f, 0.8873076923f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.04861111111f, 0.8673076923f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.05555555556f, 0.7692307692f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.0625f, 0.6923076923f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.06944444444f, 0.6153846154f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.08333333333f, 0.5f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.09722222222f, 0.4230769231f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.1111111111f, 0.3846153846f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.125f, 0.3461538462f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.1388888889f, 0.3076923077f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.1736111111f, 0.2307692308f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.2083333333f, 0.1923076923f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.2430555556f, 0.1730769231f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.2777777778f, 0.1507692308f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.3472222222f, 0.1192307692f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.4166666667f, 0.1f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.5555555556f, 0.07307692308f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.6944444444f, 0.06076923077f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.8333333333f, 0.05f, maxVolume, multiplier);
            AddCurveKey(ref ac, 0.9722222222f, 0.04307692308f, maxVolume, multiplier);
            AddCurveKey(ref ac, 1f, 0f, maxVolume, multiplier); // it's really 0.04192307692f, but we need it to be 0f
            return ac;
        }

        private static void AddCurveKey(ref AnimationCurve curve, float time, float value, float maxVolume, int multiplier)
        {
            curve.AddKey(time, Mathf.Min(value * maxVolume, value * maxVolume * multiplier));
        }

        private static AudioSource Create3dAudioSource(GameObject gameObject)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.rolloffMode = AudioRolloffMode.Custom;
            audioSource.spatialBlend = 1f;
            audioSource.dopplerLevel = 0f;
            return audioSource;
        }

        public static bool IsWalkOrRunSound(CharacterSoundType type)
        {
            return type == CharacterSoundType.Walking || type == CharacterSoundType.Running;
        }

        public static bool IsWalkOrRunInterrupt(CharacterSoundType type)
        {
            return type == CharacterSoundType.Sit || type == CharacterSoundType.Idle ||
                   type == CharacterSoundType.Passive;
        }

        public static bool IsSilentPlayerSound(CharacterSoundType type)
        {
            switch (type)
            {
                case CharacterSoundType.Swim:
                case CharacterSoundType.Treading:
                    return true;
                default:
                    return false;
            }
        }
    }
}
