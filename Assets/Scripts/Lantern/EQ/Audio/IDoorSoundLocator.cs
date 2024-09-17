namespace Lantern.EQ.Audio
{
    public interface IDoorSoundLocator
    {
        public DoorSoundData GetDoorSounds(int openType);
    }
}
