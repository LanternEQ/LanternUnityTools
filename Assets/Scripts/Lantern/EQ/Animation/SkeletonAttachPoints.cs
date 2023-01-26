using Lantern.EQ.Helpers;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    public class SkeletonAttachPoints : MonoBehaviour
    {
        [SerializeField]
        private AttachPoint _attachPoint;

        public Transform GetAttachPoint(SkeletonPoints point)
        {
            return _attachPoint.ContainsKey(point) ? _attachPoint[point] : null;
        }

#if UNITY_EDITOR
        public void FindAttachPoints()
        {
            _attachPoint = new AttachPoint();
            var skeletonRoot = transform.Find("root");

            if (skeletonRoot == null)
            {
                Debug.LogError($"Cannot find skeleton root for model {gameObject.name}");
                return;
            }

            FindAndAddPoint(skeletonRoot, "r_point", SkeletonPoints.HandRight);
            FindAndAddPoint(skeletonRoot, "l_point", SkeletonPoints.HandLeft);
            FindAndAddPoint(skeletonRoot, "he", SkeletonPoints.Head);
            FindAndAddPoint(skeletonRoot, "shield_point", SkeletonPoints.Shield);
            FindAndAddPoint(skeletonRoot, "bo_l", SkeletonPoints.FootLeft);
            FindAndAddPoint(skeletonRoot, "bo_r", SkeletonPoints.FootRight);

            // For the center, it's best to use the first child bone
            // As a fallback, we can use the skeleton root
            if (skeletonRoot.childCount != 0)
            {
                AddSkeletonPoint(skeletonRoot.GetChild(0), SkeletonPoints.Center);
            }
            else
            {
                AddSkeletonPoint(skeletonRoot, SkeletonPoints.Center);
            }
        }

        private void FindAndAddPoint(Transform skeletonRoot, string boneName, SkeletonPoints rightHand)
        {
            var bone = skeletonRoot.FindChildRecursive(boneName);

            if (bone == null)
            {
                return;
            }

            _attachPoint[rightHand] = bone;
        }

        private void AddSkeletonPoint(Transform skeletonRoot, SkeletonPoints rightHand)
        {
            _attachPoint[rightHand] = skeletonRoot;
        }
    #endif
    }
}
