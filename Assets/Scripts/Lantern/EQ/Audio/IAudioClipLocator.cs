using UnityEngine;

namespace Lantern.EQ.Audio
{
    public interface IAudioClipLocator
    {
        public AudioClip GetAudioClip(string clipName);
    }
}
