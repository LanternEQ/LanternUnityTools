using System.Collections.Generic;
using Lantern.EQ.AssetBundles;

namespace Lantern.EQ.Editor.EqAssetCopy
{
    public class SpriteSheetsToCreate
    {
        public string EqFolder;
        public LanternAssetBundleId AssetBundle;
        public List<string> AssetsToPack;           // for specifying explicit filenames
        public string AssetBase;                    // the base to which the indices are added
        public List<int> AssetIndices;              // for specifying indices
        public int Padding;
        public string SheetFileName;
    }
}
