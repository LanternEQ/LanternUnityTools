using System.Collections.Generic;
using Lantern.EQ.AssetBundles;

namespace Lantern.EQ.Editor.EqAssetCopy
{
    public class AssetToCopy
    {
        public string EqFolder;
        public LanternAssetBundleId AssetBundle;
        public List<string> AssetsToCopy;
        public AssetImportType AssetImportType;
    }
}
