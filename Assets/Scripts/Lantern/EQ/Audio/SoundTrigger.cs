using System.Collections;
using Lantern.EQ.Trigger;
using UnityEngine;

namespace Lantern.EQ.Audio
{
    [RequireComponent(typeof(AudioSource))]

    public abstract class SoundTrigger : SphereTrigger
    {
        [SerializeField]
        protected AudioSource AudioSource;

        protected Coroutine LoopCoroutine;
        protected bool IsPlayerInside;

        public abstract void FindAudioClips(IAudioClipLocator locator);

        protected abstract bool HasCooldown();
        protected abstract IEnumerator DoLoop();

        protected override void OnEnter()
        {
            IsPlayerInside = true;

            if (HasCooldown())
            {
                AudioSource.loop = false;
                LoopCoroutine = StartCoroutine(DoLoop());
            }
            else
            {
                AudioSource.loop = true;
                AudioSource.Play();
            }
        }

        protected override void OnExit()
        {
            IsPlayerInside = false;
            AudioSource.Stop();
        }
    }
}
