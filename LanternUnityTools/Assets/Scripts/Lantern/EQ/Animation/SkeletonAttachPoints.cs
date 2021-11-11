using System;
using Infastructure.SerializableDictionary;
using Lantern.EQ.Animation;
using UnityEngine;

namespace Lantern
{
    public class SkeletonAttachPoints : MonoBehaviour
    {
        [Serializable]
        public class AttachPoints : SerializableDictionary<SkeletonPoints, Transform>
        {
        }
    
    

        [SerializeField] private AttachPoints _attachPoints;
    
        // Editor only
        public void FindAttachPoints()
        {
            _attachPoints = new AttachPoints();
            var skeletonRoot = transform.Find("root");

            if (skeletonRoot == null)
            {
                Debug.LogError($"Cannot find skeleton root for model {gameObject.name}");
                return;
            }

            FindAndAddPoint(skeletonRoot, "r_point", SkeletonPoints.HandRight);
            FindAndAddPoint(skeletonRoot, "l_point", SkeletonPoints.HandLeft);
            FindAndAddPoint(skeletonRoot, "head_point", SkeletonPoints.Head);
            FindAndAddPoint(skeletonRoot, "shield_point", SkeletonPoints.Shield);
            FindAndAddPoint(skeletonRoot, "ft_l", SkeletonPoints.FootLeft);
            FindAndAddPoint(skeletonRoot, "ft_l", SkeletonPoints.FootRight);
            AddSkeletonPoint(skeletonRoot, SkeletonPoints.Center);
        }

        public Transform GetAttachPoint(SkeletonPoints point)
        {
            return _attachPoints.ContainsKey(point) ? _attachPoints[point] : null;
        }

        private void FindAndAddPoint(Transform skeletonRoot, string boneName, SkeletonPoints rightHand)
        {
            var bone = skeletonRoot.FindChildRecursive(boneName);

            if (bone == null)
            {
                return;
            }
        
            _attachPoints[rightHand] = bone;
        }
    
        private void AddSkeletonPoint(Transform skeletonRoot, SkeletonPoints rightHand)
        {
            _attachPoints[rightHand] = skeletonRoot;
        }
    }
}