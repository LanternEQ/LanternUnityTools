using System.Collections;
using Lantern.EQ.Data;
using Lantern.EQ.Lantern;
using UnityEngine;

namespace Lantern.EQ.Audio
{
    public class SoundTrigger2d : SoundTrigger
    {
        [SerializeField]
        private Sound2dData _soundData;

        private AudioClip _clipDay;
        private AudioClip _clipNight;
        private bool _isDay;

        private void Awake()
        {
            _collider.enabled = false;
        }

        public override void FindAudioClips(IAudioClipLocator locator)
        {
            _clipDay = locator.GetAudioClip(_soundData.ClipNameDay);
            _clipNight = locator.GetAudioClip(_soundData.ClipNameNight);
            _collider.enabled = true;
        }

        protected override bool HasCooldown()
        {
            var clip = AudioSource.clip;
            return clip == _isDay && _soundData.CooldownDay > 0 ||
                   clip == !_isDay && _soundData.CooldownNight > 0;
        }

        protected override IEnumerator DoLoop()
        {
            // If audio source is playing, ignore
            if (!AudioSource.isPlaying)
            {
                SetClipAndVolume();
                AudioSource.Play();
            }

            // TODO: Random
            yield return new WaitForSeconds(_isDay ? _soundData.CooldownDay : _soundData.CooldownNight);
            LoopCoroutine = IsPlayerInside ? StartCoroutine(DoLoop()) : null;
        }

        private void SetClipAndVolume()
        {
            AudioSource.clip = _isDay ? _clipDay : _clipNight;
            AudioSource.volume = (_isDay ? _soundData.VolumeDay : _soundData.VolumeNight) * EqConstants.AudioVolumeSound2d;
        }

        public void SetDayNight(bool isDayTime)
        {
            _isDay = isDayTime;
            SetClipAndVolume();

            if (IsPlayerInside && AudioSource.clip != null)
            {
                AudioSource.Play();
            }
            else
            {
                AudioSource.Stop();
            }
        }

#if UNITY_EDITOR
        public void SetData(Sound2dData sound2dData, string tag, float radius)
        {
            _soundData = sound2dData;
            _tag = tag;
            AudioSource.maxDistance = radius * LanternConstants.WorldScale; // 2d sounds don't have a radius, but it shows the 3d radius anyway
            _collider.radius = radius; // colliders scale with world automatically
        }
#endif
    }
}
