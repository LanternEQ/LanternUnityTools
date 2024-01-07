using Lantern.EQ.Trigger;
using UnityEngine;

namespace Lantern.EQ.Audio
{
    public class MusicTrigger : SphereTrigger
    {
        [SerializeField]
        private MusicData _musicData;

        private IMusicInvoker _invoker;

#if UNITY_EDITOR
        public void SetData(string tag, float radius, MusicData musicData)
        {
            _tag = tag;
            _collider.radius = radius;
            _musicData = musicData;
        }
#endif

        private void Awake()
        {
            _collider.enabled = false;
        }

        public void SetInvoker(IMusicInvoker musicInvoker)
        {
            _invoker = musicInvoker;
            _invoker.Initialize(_musicData);
            _collider.enabled = true;
        }

        protected override void OnEnter()
        {
            _invoker?.Play();
        }

        protected override void OnExit()
        {
            _invoker?.Stop();
        }
    }
}
