using System;
using Infrastructure.EQ.SerializableDictionary;

namespace Lantern.EQ.Animation
{
    [Serializable]
    public class AnimationPair : SerializableDictionary<AnimationType, AnimationType>
    {
    }
}
