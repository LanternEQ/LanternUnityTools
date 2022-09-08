using UnityEngine;

namespace Lantern.EQ.Animation
{
    /// <summary>
    /// Caches and starts the object animation when the game starts
    /// </summary>
    public class ObjectAnimation : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Animation _animation;

        private void Start()
        {
            _animation.Play();
        }

#if UNITY_EDITOR
        public void Initialize(AnimationClip clip)
        {
            _animation = GetComponent<UnityEngine.Animation>();
            _animation.clip = clip;
            _animation.wrapMode = WrapMode.Loop;
        }
#endif
    }
}
