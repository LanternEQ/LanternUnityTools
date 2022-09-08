namespace Lantern.EQ.Equipment
{
    /// <summary>
    /// A slot that can hold 3d items.
    /// Is different from SkeletonAttachPoints as multiple slots can use the same attach point.
    /// </summary>
    public enum Equipment3dSlot
    {
        MainHand = 0,
        OffHand = 1,
        Shield = 2,
        Helm = 3,
        Ranged = 4,
    }
}
