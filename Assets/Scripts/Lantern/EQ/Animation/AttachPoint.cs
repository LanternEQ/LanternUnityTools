using System;
using Infrastructure.EQ.SerializableDictionary;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    [Serializable]
    public class AttachPoint : SerializableDictionary<SkeletonPoints, Transform>
    {
    }
}
