using System.Collections;
using Lantern.EQ.Data;
using Lantern.EQ.Lantern;
using UnityEngine;

namespace Lantern.EQ.Audio
{
    public class SoundTrigger3d : SoundTrigger
    {
        [SerializeField]
        private Sound3dData _soundData;

        public override void FindAudioClips(IAudioClipLocator locator)
        {
            AudioSource.clip = locator.GetAudioClip(_soundData.ClipName);
            AudioSource.volume = _soundData.Volume * EqConstants.AudioVolumeSound3d;
        }

        protected override bool HasCooldown()
        {
            return _soundData.Cooldown > 0;
        }

        protected override IEnumerator DoLoop()
        {
            // If audio source is playing, ignore
            if (!AudioSource.isPlaying)
            {
                AudioSource.Play();
            }

            // TODO: Random
            yield return new WaitForSeconds(_soundData.Cooldown);
            LoopCoroutine = IsPlayerInside ? StartCoroutine(DoLoop()) : null;
        }

#if UNITY_EDITOR
        public void SetData(Sound3dData soundData, string tag, float radius)
        {
            _tag = tag;
            _soundData = soundData;
            AudioSource.maxDistance = radius * LanternConstants.WorldScale;
            AudioSource.rolloffMode = AudioRolloffMode.Custom;
            AudioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff,
                AudioHelper.GetSound3DFalloff(soundData.Multiplier));
            _collider.radius = radius; // colliders scale with world automatically
        }
#endif
    }
}
