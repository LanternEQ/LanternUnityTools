using System;

namespace Lantern.EQ.Audio
{
    [Flags]
    public enum ChannelFlag
    {
        Unset = 0,
        Channel1 = 1 << 0,
        Channel2 = 1 << 1,
        Channel3 = 1 << 2,
        Channel4 = 1 << 3,
        Channel5 = 1 << 4,
        Channel6 = 1 << 5,
        Channel7 = 1 << 6,
        Channel8 = 1 << 7,
        Channel9 = 1 << 8,
        Channel10 = 1 << 9,
        Channel11 = 1 << 10,
        Channel12 = 1 << 11,
        Channel13 = 1 << 12,
        Channel14 = 1 << 13,
        Channel15 = 1 << 14,
        Channel16 = 1 << 15,

        Barbarian = Channel2 | Channel3 | Channel6 | Channel8 | Channel9 | Channel10 | Channel11 | Channel13 | Channel14,
        Dark_Elf = Channel6 | Channel7 | Channel10 | Channel11 | Channel14,
        Dwarf = Channel4 | Channel5 | Channel10 | Channel11 | Channel13 | Channel15,
        Erudite = Channel1 | Channel4 | Channel5 | Channel6 | Channel8 | Channel11,
        Gnome = Channel2 | Channel3 | Channel6 | Channel10 | Channel13 | Channel15,
        Half_Elf = Channel2 | Channel3 | Channel6 | Channel7 | Channel9 | Channel11,
        Halfling = Channel1 | Channel2 | Channel3 | Channel6 | Channel15,
        High_Elf = Channel2 | Channel3 | Channel4 | Channel5 | Channel10 | Channel13 | Channel14,
        Human = Channel1 | Channel6 | Channel11,
        Iksar = Channel2 | Channel3 | Channel6 | Channel8 | Channel9 | Channel10 | Channel14,
        Ogre = Channel11 | Channel12 | Channel13,
        Troll = Channel2 | Channel3 | Channel4 | Channel5 | Channel10 | Channel12,
        Wood_Elf = Channel1 | Channel2 | Channel3 | Channel7 | Channel11 | Channel13
    }
}
