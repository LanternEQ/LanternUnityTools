namespace Lantern.EQ.Audio
{
    public interface ICharacterSoundLocator
    {
        public CharacterSoundData GetCharacterSounds(int raceId, int classId, int textureId, float scaleFactor = 6f);
    }
}
