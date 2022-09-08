using System;
using Infrastructure.EQ.SerializableDictionary;
using UnityEngine;

namespace Lantern.EQ.Equipment
{
    [Serializable]
    public class EquipmentTextures : SerializableDictionary<SkinnedMeshRenderer, Skins>
    {
    }
}
