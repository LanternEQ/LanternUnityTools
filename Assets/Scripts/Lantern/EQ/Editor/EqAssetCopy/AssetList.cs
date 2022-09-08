using System.Collections.Generic;
using Lantern.EQ.AssetBundles;
using UnityEngine;

namespace Lantern.EQ.Editor.EqAssetCopy
{
    public static class AssetList
    {
        public static List<AssetToCopy> AssetsToCopy = new List<AssetToCopy>()
        {
            new AssetToCopy
            {
                EqFolder = "bmpwad", AssetBundle = LanternAssetBundleId.CharacterSelect_Classic, AssetsToCopy =
                    new List<string>
                    {
                        "ccreate.png",
                        "ccreate2.png",
                        "cselect.png",
                        "godscreen.png",
                        "locblue.png",
                        "locgold.png",
                        "locred.png",
                        "bugrpt.png",
                    },
                AssetImportType = AssetImportType.Sprite
            },
            new AssetToCopy
            {
                EqFolder = "bmpwad3", AssetBundle = LanternAssetBundleId.CharacterSelect_Classic, AssetsToCopy =
                    new List<string>
                    {
                        "locnewb.png",
                        "locnewr.png",
                        "locnewy.png",
                        "ccreate1b.png",
                        "ccreate2dialoga.png",
                    },
                AssetImportType = AssetImportType.Sprite
            },
            new AssetToCopy
            {
                EqFolder = "bmpwad", AssetBundle = LanternAssetBundleId.Startup, AssetsToCopy = new List<string>
                {
                    "logo03.png",
                },
                AssetImportType = AssetImportType.Sprite
            },
            new AssetToCopy
            {
                EqFolder = "bmpwad4", AssetBundle = LanternAssetBundleId.Startup, AssetsToCopy = new List<string>
                {
                    "eqkload.png",
                },
                AssetImportType = AssetImportType.Sprite
            },
            new AssetToCopy
            {
                EqFolder = "bmpwad5", AssetBundle = LanternAssetBundleId.Startup, AssetsToCopy = new List<string>
                {
                    "eqvload.png",
                },
                AssetImportType = AssetImportType.Sprite
            },
            new AssetToCopy
            {
                EqFolder = "bmpwad", AssetBundle = LanternAssetBundleId.Sprites, AssetsToCopy = new List<string>
                {
                    "dragitem01.png",
                    "dragitem02.png",
                    "dragitem03.png",
                    "spelgems.png",
                    "spelicon.png",
                },
                AssetImportType = AssetImportType.SpriteSheet // converted to sprites in post processing
            },
            new AssetToCopy
            {
                EqFolder = "bmpwad2", AssetBundle = LanternAssetBundleId.Sprites, AssetsToCopy = new List<string>
                {
                    "dragitem04.png",
                },
                AssetImportType = AssetImportType.SpriteSheet // converted to sprites in post processing
            },
        };

        public static List<SpritesSheetsToSplice> SpriteSheetsToSplice = new List<SpritesSheetsToSplice>
        {
            new SpritesSheetsToSplice
            {
                AssetBundle = LanternAssetBundleId.Sprites,
                AssetToSplice = new List<string>
                {
                    "dragitem01.png",
                    "dragitem02.png",
                    "dragitem03.png",
                    "dragitem04.png",
                },
                CellSize = new Vector2Int(40, 40),
                KeepEmptyRects = true
            },
            new SpritesSheetsToSplice
            {
                AssetBundle = LanternAssetBundleId.Sprites,
                AssetToSplice = new List<string>
                {
                    "spelgems.png",
                },
                CellSize = new Vector2Int(31, 23),
                KeepEmptyRects = false,
                CellsToRemove = new List<int> { 7, 15, 17 }
            },
            new SpritesSheetsToSplice
            {
                AssetBundle = LanternAssetBundleId.Sprites,
                AssetToSplice = new List<string>
                {
                    "spelicon.png",
                },
                CellSize = new Vector2Int(40, 40),
                KeepEmptyRects = false,
                CellsToRemove = new List<int>{10}
            }
        };

        public static List<SpriteSheetsToCreate> SpriteSheetsToCreate = new List<SpriteSheetsToCreate>
        {
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "gena",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17, 20, 30, 40
                },
                SheetFileName = "gena.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genb",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17, 20, 30, 40
                },
                SheetFileName = "genb.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genc",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17, 20, 30, 40
                },
                SheetFileName = "genc.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "gend",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17, 20, 30, 40
                },
                SheetFileName = "gend.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "gene",
                AssetIndices = new List<int>
                {
                    1
                },
                SheetFileName = "gene.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "geng",
                AssetIndices = new List<int>
                {
                    0
                },
                SheetFileName = "geng.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genh",
                AssetIndices = new List<int>
                {
                    0, 1
                },
                SheetFileName = "genh.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "geni",
                AssetIndices = new List<int>
                {
                    0, 1
                },
                SheetFileName = "geni.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genj",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17
                },
                SheetFileName = "genj.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genk",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17
                },
                SheetFileName = "genk.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genl",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7
                },
                SheetFileName = "genl.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genm",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7
                },
                SheetFileName = "genm.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genn",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7
                },
                SheetFileName = "genn.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "geno",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7
                },
                SheetFileName = "geno.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genp",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7
                },
                SheetFileName = "genp.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genq",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7
                },
                SheetFileName = "genq.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genr",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17
                },
                SheetFileName = "genr.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "gens",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17
                },
                SheetFileName = "gens.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "gent",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7
                },
                SheetFileName = "gent.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genu",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17
                },
                SheetFileName = "genu.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genv",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7
                },
                SheetFileName = "genv.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genw",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17
                },
                SheetFileName = "genw.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genx",
                AssetIndices = new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7
                },
                SheetFileName = "genx.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "geny",
                AssetIndices = new List<int>
                {
                    10, 11, 12, 13, 14, 15, 16, 17, 20, 30, 40
                },
                SheetFileName = "geny.png"
            },
            new SpriteSheetsToCreate
            {
                EqFolder = "equipment/textures",
                AssetBundle = LanternAssetBundleId.Sprites,
                Padding = 10,
                AssetBase = "genz",
                AssetIndices = new List<int>
                {
                    0, 1
                },
                SheetFileName = "genz.png"
            },
        };
    }
}
