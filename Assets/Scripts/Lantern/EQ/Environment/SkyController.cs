using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lantern.EQ.Environment
{
    public class SkyController : MonoBehaviour
    {
        [Serializable]
        private class SkyGroup
        {
            public List<GameObject> GroupObjects;
            public UnityEngine.Animation ObjectsAnimation;
        }

        [FormerlySerializedAs("_skyGroups2")] [SerializeField] private List<SkyGroup> _skyGroups = new List<SkyGroup>();
        [SerializeField] private List<GameObject> _objectPool = new List<GameObject>();
        [SerializeField] private Transform _cameraFollow;
        [SerializeField] private Vector3 _cameraFollowOffset;

        private List<SkyLayer> _layers;

        private float _secondsPerDay;
        private int _currentSky;
        private Quaternion _fixedRotation;
        private UnityEngine.Animation _currentAnimation;

        private void Awake()
        {
            _fixedRotation = transform.rotation;
            _layers = GetComponentsInChildren<SkyLayer>(true).ToList();
        }

        public void SetActiveCameraTransform(Transform cameraTransform, bool instant = false)
        {
            if (_cameraFollow == cameraTransform)
            {
                return;
            }

            _cameraFollow = cameraTransform;

            if (_layers == null)
            {
                _layers = GetComponentsInChildren<SkyLayer>(true).ToList();
            }

            if (instant)
            {
                UpdateSkyPosition();
            }
        }

        public void SetSecondsPerDay(float seconds)
        {
            _secondsPerDay = seconds;
        }

        private void SetNewSkyIndex(int index)
        {
            if (index < 0 || index >= _skyGroups.Count)
            {
                return;
            }

            foreach (var go in _objectPool)
            {
                go.SetActive(false);
            }

            foreach (var skyObject in _skyGroups[index].GroupObjects)
            {
                skyObject.SetActive(true);
            }

            _currentAnimation = _skyGroups[index].ObjectsAnimation;
            SetSkyAnimationState(index);
        }

#if UNITY_EDITOR
        public void AddSkyGroup(List<GameObject> activeObjects, UnityEngine.Animation animation)
        {
            _skyGroups.Add(new SkyGroup
            {
                GroupObjects = activeObjects,
                ObjectsAnimation = animation
            });

            foreach (var ao in activeObjects)
            {
                if (!_objectPool.Contains(ao))
                {
                    _objectPool.Add(ao);
                }
            }
        }
#endif

        public void UpdateTimeLate(float deltaTime, float currentTime)
        {
            foreach (var sl in _layers)
            {
                if (sl.gameObject.activeSelf)
                {
                    sl.UpdateTimeLate(deltaTime, currentTime);
                }
            }
        }

        public void UpdateTime(float time)
        {
            if (_currentAnimation == null)
            {
                return;
            }

            var animName = _currentAnimation.clip.name;

            // If the animation falls out of sync, we fix it here
            if (Mathf.Abs(_currentAnimation[animName].normalizedTime % 1.0f - time) > 0.01f)
            {
                FixSkyAnimation(time);
            }
        }

        private void FixSkyAnimation(float time)
        {
            var animName = _currentAnimation.clip.name;
            _currentAnimation[animName].normalizedTime = time;
        }

        public void SetEnabledSky(int skyIndex)
        {
            _currentSky = skyIndex;
            SetNewSkyIndex(skyIndex);
            SetSkyAnimationState(1);
        }

        private void SetSkyAnimationState(int index)
        {
            if (index < 0 || index >= _skyGroups.Count)
            {
                return;
            }

            var animation = _skyGroups[index].ObjectsAnimation;
            if (animation == null)
            {
                return;
            }

            float animationLength = 4f;
            float playSpeed = _secondsPerDay;
            float multiplier = animationLength / playSpeed;
            var clipName = animation.clip.name;
            animation[clipName].speed = multiplier;
            animation.Play();
        }

        public void UpdateSkyPosition()
        {
            if (_cameraFollow == null)
            {
                return;
            }

            var selfTransform = transform;
            selfTransform.position = _cameraFollow.position + _cameraFollowOffset;
            selfTransform.rotation = _fixedRotation;
        }

        public int GetSkyObjectCount()
        {
            return _skyGroups[_currentSky].GroupObjects.Count;
        }
    }
}
