using System;
using Infrastructure.EQ.SerializableDictionary;

namespace Lantern.EQ.Animation
{
    [Serializable]
    public class AnimationClipMapping : SerializableDictionary<AnimationType, string>
    {
    }
}
