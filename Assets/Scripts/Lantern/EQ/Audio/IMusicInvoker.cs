namespace Lantern.EQ.Audio
{
    public interface IMusicInvoker
    {
        void Initialize(MusicData musicData);
        void Play();
        void Stop();
    }
}
