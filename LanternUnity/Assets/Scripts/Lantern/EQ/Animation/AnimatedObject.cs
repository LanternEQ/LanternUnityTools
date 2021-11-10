using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    public class AnimatedObject : MonoBehaviour
    {
        [SerializeField]
        private List<AnimationClip> _animations = new List<AnimationClip>();
        
        private void Start()
        {
            UnityEngine.Animation anim = GetComponent<UnityEngine.Animation>();
            anim.clip = _animations.FirstOrDefault();
            anim.wrapMode = WrapMode.Loop;
            anim.Play();
        }

        public void AddAnimationClip(AnimationClip clip)
        {
            _animations.Add(clip);
        }
    }
}
