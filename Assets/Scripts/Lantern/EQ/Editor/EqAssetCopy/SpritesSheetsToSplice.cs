using System.Collections.Generic;
using Lantern.EQ.AssetBundles;
using UnityEngine;

namespace Lantern.EQ.Editor.EqAssetCopy
{
    public class SpritesSheetsToSplice
    {
        public LanternAssetBundleId AssetBundle;
        public List<string> AssetToSplice;
        public Vector2Int CellSize;
        public bool KeepEmptyRects;
        public List<int> CellsToRemove;
    }
}
