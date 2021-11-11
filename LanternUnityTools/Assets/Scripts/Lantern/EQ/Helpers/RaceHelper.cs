using System;
using System.Collections.Generic;
using System.IO;
using Lantern.Editor;
using Lantern.EQ;
using UnityEngine;

// TODO: Move this elsewhere

namespace Lantern.Helpers
{
    public static class RaceHelper
    {
        private static Dictionary<int, RaceData> RaceValues = new Dictionary<int, RaceData>();
        
        public static float GetBoundingRadius(int id)
        {
            if (RaceValues == null || RaceValues.Count == 0)
            {
                ParseRaceDataFromFile();
            }

            if (RaceValues == null || !RaceValues.ContainsKey(id))
            {
                return 1.0f;
            }

            return RaceValues[id].BoundingRadius;
        }

        private static void ParseRaceDataFromFile()
        {
            var modelSoundsAssetPath = Application.streamingAssetsPath + "/ClientData/RaceData.csv";
            var textLines = File.ReadAllText(modelSoundsAssetPath);
            var lines = TextParser.ParseTextByDelimitedLines(textLines, ',');

            for (int i = 0; i < lines.Count; ++i)
            {
                if (i == 0)
                {
                    continue;
                }

                int id = Convert.ToInt32(lines[i][0]);
                
                RaceValues.Add(id, new RaceData
                {
                    Id = id,
                    Name = lines[i][1],
                    Male = lines[i][2],
                    Female = lines[i][3],
                    Neutral = lines[i][4],
                    Height = float.Parse(lines[i][5]),
                    BoundingRadius = float.Parse(lines[i][6]),
                    ScaleFactor = float.Parse(lines[i][7]),
                    Size = float.Parse(lines[i][8])
                });
            }        
        }

        public class RaceData
        {
            public int Id;
            public string Name;
            public string Male;
            public string Female;
            public string Neutral;
            public float Height;
            public float BoundingRadius;
            public float ScaleFactor;
            public float Size;
        }

        public static string DefaultMaleModel = "HUM";
        public static string DefaultFemaleModel = "HUF";
        public static string DefaultNeutralModel = "HUM";
        public static float DefaultSize = 6f;
        public static float GetRaceDefaultSize(int raceId)
        {
            switch (raceId)
            {
                case 1:
                case 3:
                case 5:
                    return 6;
                case 2:
                    return 7;
                case 4:
                case 6:
                    return 5;
                // TODO: Verify
                case 7:
                    return 5.30000019073486f;
                case 8:
                    return 4;
                case 9:
                    return 7.5f;
                case 10:
                    return 8.8f;
                case 11:
                    return 3.5f;
                case 12:
                    return 3;
            }
            
            return 6;
        }
        
        public static int GetRaceIdFromCharacterCreateId(RaceIdDb raceIdDb)
        {
            switch (raceIdDb)
            {
                case RaceIdDb.Human:
                    return 1;
                case RaceIdDb.Barbarian:
                    return 2;
                case RaceIdDb.Erudite:
                    return 3;
                case RaceIdDb.Wood_Elf:
                    return 4;
                case RaceIdDb.High_Elf:
                    return 5;
                case RaceIdDb.Dark_Elf:
                    return 6;
                case RaceIdDb.Half_Elf:
                    return 7;
                case RaceIdDb.Dwarf:
                    return 8;
                case RaceIdDb.Troll:
                    return 9;
                case RaceIdDb.Ogre:
                    return 10;
                case RaceIdDb.Halfling:
                    return 11;
                case RaceIdDb.Gnome:
                    return 12;
                case RaceIdDb.Iksar:
                    return 128;
            }

            return 0;
        }

        public static bool IsPlayableRace(string raceId)
        {
            switch (raceId.ToUpper())
            {
                case "HUM":
                case "HUF":
                case "BAM":
                case "BAF":
                case "ERM":
                case "ERF":
                case "ELM":
                case "ELF":
                case "HIM":
                case "HIF":
                case "DAM":
                case "DAF":
                case "HAM":
                case "HAF":
                case "DWM":
                case "DWF":
                case "TRM":
                case "TRF":
                case "OGM":
                case "OGF":
                case "HOM":
                case "HOF":
                case "GNM":
                case "GNF":
                case "IKM":
                case "IKF":
                    return true;
                default:
                    return false;
            }
        }

        public class RaceGenderModels
        {
            public string Male { get; set; }
            public string Female { get; set; }
            public string Neutral { get; set; }
        }

        public static bool IsBoat(int raceId)
        {
            switch (raceId)
            {
                case 72:  // SHIP
                case 73:  // LAUNCH
                case 114: // GSP
                case 141: // BOAT
                    return true;
                default:
                    return false;
            }
        }

        private static Dictionary<int, RaceGenderModels> RaceIds = new Dictionary<int, RaceGenderModels>()
        {
            {1, new RaceGenderModels {Male = "HUM", Female = "HUF"}},
            {2, new RaceGenderModels {Male = "BAM", Female = "BAF"}},
            {3, new RaceGenderModels {Male = "ERM", Female = "ERF"}},
            {4, new RaceGenderModels {Male = "ELM", Female = "ELF"}},
            {5, new RaceGenderModels {Male = "HIM", Female = "HIF"}},
            {6, new RaceGenderModels {Male = "DAM", Female = "DAF"}},
            {7, new RaceGenderModels {Male = "HAM", Female = "HAF"}},
            {8, new RaceGenderModels {Male = "DWM", Female = "DWF"}},
            {9, new RaceGenderModels {Male = "TRM", Female = "TRF"}},
            {10, new RaceGenderModels {Male = "OGM", Female = "OGF"}},
            {11, new RaceGenderModels {Male = "HOM", Female = "HOF"}},
            {12, new RaceGenderModels {Male = "GNM", Female = "GNF"}},
            {13, new RaceGenderModels {Neutral = "AVI"}},
            {14, new RaceGenderModels {Neutral = "WER"}},
            {15, new RaceGenderModels {Male = "BRM", Female = "BRF"}},
            {16, new RaceGenderModels {Neutral = "CEN"}},
            {17, new RaceGenderModels {Male = "GOM", Neutral = "GOL"}},
            {18, new RaceGenderModels {Neutral = "GIA"}},
            {19, new RaceGenderModels {Neutral = "TRK"}},
            {20, new RaceGenderModels {Neutral = "VST"}},
            {21, new RaceGenderModels {Neutral = "BEH"}},
            {22, new RaceGenderModels {Neutral = "BET"}},
            {23, new RaceGenderModels {Male = "CPM", Female = "CPF"}},
            {24, new RaceGenderModels {Neutral = "FIS"}},
            {25, new RaceGenderModels {Female = "FAF"}},
            {26, new RaceGenderModels {Neutral = "FRO"}},
            {27, new RaceGenderModels {Neutral = "FRG"}},
            {28, new RaceGenderModels {Neutral = "FUN"}},
            {29, new RaceGenderModels {Male = "GAM", Neutral = "GAR"}},
            {30, new RaceGenderModels {Neutral = "BEH"}},
            {31, new RaceGenderModels {Neutral = "CUB"}},
            {33, new RaceGenderModels {Neutral = "GHU"}},
            {34, new RaceGenderModels {Neutral = "BAT"}},
            {36, new RaceGenderModels {Neutral = "RAT"}},
            {37, new RaceGenderModels {Neutral = "SNA"}},
            {38, new RaceGenderModels {Neutral = "SPI"}},
            {39, new RaceGenderModels {Neutral = "GNN"}},
            {40, new RaceGenderModels {Neutral = "GOB"}},
            {41, new RaceGenderModels {Neutral = "GOR"}},
            {42, new RaceGenderModels {Neutral = "WOL", Female = "WOF"}},
            {43, new RaceGenderModels {Neutral = "BEA"}},
            {44, new RaceGenderModels {Male = "FPM"}},
            {45, new RaceGenderModels {Neutral = "DML"}},
            {46, new RaceGenderModels {Neutral = "IMP"}},
            {47, new RaceGenderModels {Neutral = "GRI"}},
            {48, new RaceGenderModels {Neutral = "KOB"}},
            {49, new RaceGenderModels {Neutral = "DRA"}},
            {50, new RaceGenderModels {Male = "LIM", Female = "LIF"}},
            {51, new RaceGenderModels {Neutral = "LIZ"}},
            {52, new RaceGenderModels {Male = "MIM"}},
            {53, new RaceGenderModels {Neutral = "MIN"}},
            {54, new RaceGenderModels {Neutral = "ORC"}},
            {55, new RaceGenderModels {Male = "BGM"}},
            {56, new RaceGenderModels {Female = "PIF"}},
            {57, new RaceGenderModels {Male = "DRM", Female = "DRF"}},
            {58, new RaceGenderModels {Neutral = "SOL"}},
            {59, new RaceGenderModels {Neutral = "BGG"}},
            {60, new RaceGenderModels {Neutral = "SKE"}},
            {61, new RaceGenderModels {Neutral = "SHA"}},
            {62, new RaceGenderModels {Neutral = "TUN"}},
            {63, new RaceGenderModels {Neutral = "TIG"}},
            {64, new RaceGenderModels {Neutral = "TRE"}},
            {65, new RaceGenderModels {Male = "DVM"}},
            {66, new RaceGenderModels {Neutral = "RAL"}},
            {67, new RaceGenderModels {Male = "HHM"}},
            {68, new RaceGenderModels {Neutral = "TEN"}},
            {69, new RaceGenderModels {Neutral = "WIL"}},
            {70, new RaceGenderModels {Male = "ZOM", Female = "ZOF"}},
            {71, new RaceGenderModels {Male = "QCM", Female = "QCF"}},
            {72, new RaceGenderModels {Male = "SHIP", Female = "SHIP", Neutral = "SHIP"}},
            {73, new RaceGenderModels {Male = "LAUNCH", Female = "LAUNCH", Neutral = "LAUNCH"}},
            {74, new RaceGenderModels {Neutral = "PIR"}},
            {75, new RaceGenderModels {Neutral = "ELE"}},
            {76, new RaceGenderModels {Neutral = "PUM"}},
            {77, new RaceGenderModels {Male = "NGM"}},
            {78, new RaceGenderModels {Male = "EGM"}},
            {79, new RaceGenderModels {Neutral = "BIX"}},
            {80, new RaceGenderModels {Neutral = "REA"}},
            {81, new RaceGenderModels {Male = "RIM", Female = "RIF"}},
            {82, new RaceGenderModels {Neutral = "SCA"}},
            {83, new RaceGenderModels {Neutral = "SKU"}},
            {85, new RaceGenderModels {Neutral = "SPE"}},
            {86, new RaceGenderModels {Neutral = "SPH"}},
            {87, new RaceGenderModels {Neutral = "ARM"}},
            {88, new RaceGenderModels {Male = "CLM", Female = "CLF", Neutral = "CL"}},
            {89, new RaceGenderModels {Neutral = "DRK"}},
            {90, new RaceGenderModels {Male = "HLM", Female = "HLF"}},
            {91, new RaceGenderModels {Neutral = "ALL"}},
            {92, new RaceGenderModels {Male = "GRM", Female = "GRF"}},
            {93, new RaceGenderModels {Male = "OKM", Female = "OKF"}},
            {94, new RaceGenderModels {Male = "KAM", Female = "KAF", Neutral = "KA"}},
            {95, new RaceGenderModels {Neutral = "CAZ"}},
            {96, new RaceGenderModels {Neutral = "COC"}},
            {98, new RaceGenderModels {Male = "VSM", Female = "VSF"}},
            {99, new RaceGenderModels {Neutral = "DEN"}},
            {100, new RaceGenderModels {Neutral = "DER"}},
            {101, new RaceGenderModels {Neutral = "EFR"}}, 
            {102, new RaceGenderModels {Neutral = "FRT"}},
            {103, new RaceGenderModels {Neutral = "KED"}},
            {104, new RaceGenderModels {Neutral = "LEE"}},
            {105, new RaceGenderModels {Neutral = "SWO"}},
            {106, new RaceGenderModels {Male = "FEM"}},
            {107, new RaceGenderModels {Neutral = "MAM"}},
            {108, new RaceGenderModels {Neutral = "EYE"}},
            {109, new RaceGenderModels {Neutral = "WAS"}},
            {110, new RaceGenderModels {Neutral = "MER"}},
            {111, new RaceGenderModels {Neutral = "HAR"}},
            {112, new RaceGenderModels {Male = "GFM", Female = "GFF"}},
            {113, new RaceGenderModels {Neutral = "DRI"}},
            {114, new RaceGenderModels {Neutral = "GSP"}},
            {116, new RaceGenderModels {Neutral = "SEA"}},
            {117, new RaceGenderModels {Male = "GDM"}},
            {118, new RaceGenderModels {Male = "GEM", Female = "GEF"}},
            {119, new RaceGenderModels {Neutral = "STC"}},
            {120, new RaceGenderModels {Neutral = "WOE"}},
            {121, new RaceGenderModels {Neutral = "GRG"}},
            {122, new RaceGenderModels {Neutral = "DRU"}},
            {123, new RaceGenderModels {Neutral = "INN"}},
            {124, new RaceGenderModels {Neutral = "UNI"}},
            {125, new RaceGenderModels {Neutral = "PEG"}},
            {126, new RaceGenderModels {Neutral = "DJI"}},
            {127, new RaceGenderModels {Male = "IVM"}},
            {128, new RaceGenderModels {Male = "IKM", Female = "IKF"}},
            {129, new RaceGenderModels {Neutral = "SCR"}},
            {131, new RaceGenderModels {Neutral = "SRW"}},
            {133, new RaceGenderModels {Neutral = "LYC"}},
            {134, new RaceGenderModels {Neutral = "MOS"}},
            {135, new RaceGenderModels {Neutral = "RHI"}},
            {136, new RaceGenderModels {Neutral = "XAL"}},
            {137, new RaceGenderModels {Neutral = "KGO"}},
            {138, new RaceGenderModels {Neutral = "YET"}},
            {139, new RaceGenderModels {Male = "ICM", Female = "ICF", Neutral = "ICN"}},
            {140, new RaceGenderModels {Neutral = "FGI"}},
            {141, new RaceGenderModels {Neutral = "BOAT"}},
            {144, new RaceGenderModels {Neutral = "BRN"}},
            {145, new RaceGenderModels {Neutral = "GOO"}},
            {146, new RaceGenderModels {Neutral = "SSN"}},
            {147, new RaceGenderModels {Male = "SIM"}},
            {148, new RaceGenderModels {Neutral = "BAC"}},
            {149, new RaceGenderModels {Neutral = "ISC"}},
            {150, new RaceGenderModels {Neutral = "ERO"}},
            {151,new RaceGenderModels{Neutral = "TRI"}},
            {153,new RaceGenderModels{Neutral = "BRI"}},
            {154,new RaceGenderModels{Female = "FDF",Neutral = "FDR"}},
            {155,new RaceGenderModels{Neutral = "SSK"}},
            //{156,new RaceGenderModels{Male = "VRM",Female = "VRF"}},
            {156,new RaceGenderModels{Male = "VRF", Neutral = "VRM"}},
            {157,new RaceGenderModels{Neutral = "WYV"}},
            {158,new RaceGenderModels{Neutral = "WUR"}},
            {159,new RaceGenderModels{Neutral = "DEV"}},
            {160,new RaceGenderModels{Neutral = "IKG"}},
            {161,new RaceGenderModels{Neutral = "IKS"}},
            {162,new RaceGenderModels{Neutral = "MEP"}},
            {163,new RaceGenderModels{Neutral = "RAP"}},
            {164,new RaceGenderModels{Neutral = "SGO"}},
            {165,new RaceGenderModels{Neutral = "SED"}},
            {166,new RaceGenderModels{Neutral = "IKH"}},
            {167,new RaceGenderModels{Neutral = "SUC"}},
            {168,new RaceGenderModels{Neutral = "FMO"}},
            {169,new RaceGenderModels{Neutral = "BTM"}},
            {170,new RaceGenderModels{Neutral = "SDE"}},
            {171,new RaceGenderModels{Neutral = "DIW"}},
            {172,new RaceGenderModels{Neutral = "MTC"}},
            {173,new RaceGenderModels{Neutral = "TOT"}},
            {174,new RaceGenderModels{Neutral = "SPC"}},
            {175,new RaceGenderModels{Neutral = "ENA"}},
            {176,new RaceGenderModels{Neutral = "SBU"}},
            {177,new RaceGenderModels{Neutral = "WAL"}},
            {178,new RaceGenderModels{Neutral = "RGM"}},
            {181,new RaceGenderModels{Neutral = "YAK"}},
            {183,new RaceGenderModels{Male = "COM",Female = "COF",Neutral = "COK"}},
            {184,new RaceGenderModels{Neutral = "DR2"}},
            {185,new RaceGenderModels{Neutral = "HAG"}},
            {187,new RaceGenderModels{Neutral = "SIR"}},
            {188,new RaceGenderModels{Neutral = "FSG"}},
            {189,new RaceGenderModels{Male = "STM", Neutral = "STG"}},
            {190,new RaceGenderModels{Neutral = "OTM"}},
            {191,new RaceGenderModels{Neutral = "WLM"}},
            {192,new RaceGenderModels{Neutral = "CCD"}},
            {193,new RaceGenderModels{Neutral = "ABH"}},
            {194,new RaceGenderModels{Neutral = "STU"}},
            {195,new RaceGenderModels{Neutral = "BWD"}},
            {196,new RaceGenderModels{Neutral = "GDR"}},
            {198,new RaceGenderModels{Neutral = "PRI"}},
        };

        public static string GetModelNameFromRaceAndGender(int raceId, int genderId)
        {
            if (!RaceIds.ContainsKey(raceId))
            {
                return GetDefaultModelForGender(GetGenderFromGenderId(genderId));
            }

            var raceInfo = RaceIds[raceId];
            if ((genderId == -1 || genderId == 0) && !string.IsNullOrEmpty(raceInfo.Male))
            {
                return raceInfo.Male;
            }
            
            if ((genderId == -1 || genderId == 1) && !string.IsNullOrEmpty(raceInfo.Female))
            {
                return raceInfo.Female;
            }
            
            if ((genderId == -1 || genderId == 2) && !string.IsNullOrEmpty(raceInfo.Neutral))
            {
                return raceInfo.Neutral;
            }

            var gender = GetGenderFromGenderId(genderId);
            return GetDefaultModelForGender(gender);
        }

        public static Gender GetGenderFromGenderId(int genderId)
        {
            switch (genderId)
            {
                case 0:
                    return Gender.Male;
                case 1:
                    return Gender.Female;
                case 2:
                    return Gender.Neutral;
            }

            return Gender.Male;
        }

        public static string GetDefaultModelForGender(Gender gender)
        {
            switch (gender)
            {
                case Gender.Female:
                    return DefaultFemaleModel;
                default:
                    return DefaultMaleModel;
            }
        }

        public static int? GetRaceIdFromModelName(string modelName)
        {
            modelName = modelName.ToLower();
            
            foreach (var id in RaceIds)
            {
                var variantNames = id.Value;

                if (variantNames == null)
                {
                    continue;
                }

                if (variantNames.Male?.ToLower() == modelName || variantNames.Female?.ToLower() == modelName ||
                    variantNames.Neutral?.ToLower() == modelName)
                {
                    return id.Key;
                }
            }

            return null;
        }

        public static Gender? GetRaceGenderFromModelName(string modelName)
        {
            modelName = modelName.ToUpper();
            foreach (var id in RaceIds)
            {
                var variantNames = id.Value;
                if (variantNames.Male == modelName)
                {
                    return Gender.Male;
                }

                if (variantNames.Female == modelName)
                {
                    return Gender.Female;
                }

                if (variantNames.Neutral == modelName)
                {
                    return Gender.Neutral;
                }
            }

            return null;
        }

        public static float GetHeightIncrementForRaceId(int raceId)
        {
            switch (raceId)
            {
                // Lava Dragon, Wurm, Ghost Dragon
                case 49:
                case 158:
                case 196:
                    return 20;
                // Ship, Launch
                case 72:
                case 73:
                    return 0.1f;
                case 141:
                    return 0.0f;
                // Eye of Zomm
                case 108:
                    return 3.0f;
                default:
                    return 0.625f;
            }
        }

        public static bool IsFixedSizeRace(int raceId)
        {
            return GetScaleIncrementForRaceId(raceId) != 0.2f;
        }

        public static float GetScaleIncrementForRaceId(int raceId)
        {
            switch (raceId)
            {
                // Lava Dragon, Launch, Wurm, Ghost Dragon
                case 49:
                case 73:
                case 158:
                case 196:
                    return 1.0f;
                // Ship
                case 72:
                    return 2.0f;
                // Eye of Zomm
                case 108:
                    return 0.05f;
                default:
                    return 0.2f;
            }
        }

        public static string GetRaceNameFromRaceId(int raceId)
        {
            if (RaceValues == null || RaceValues.Count == 0)
            {
                ParseRaceDataFromFile();
            }

            if (!RaceValues.ContainsKey(raceId))
            {
                return "NPC";
            }

            return RaceValues[raceId].Name;
        }

        public static string GetPlayableRaceModelFromRaceId(PlayableRaceId race, Gender gender)
        {
            switch (race)
            {
                case PlayableRaceId.Human:
                    return gender == Gender.Male ? "HUM" : "HUF";
                case PlayableRaceId.Barbarian:
                    return gender == Gender.Male ? "BAM" : "BAF";
                case PlayableRaceId.Erudite:
                    return gender == Gender.Male ? "ERM" : "ERF";
                case PlayableRaceId.Wood_Elf:
                    return gender == Gender.Male ? "ELM" : "ELF";
                case PlayableRaceId.High_Elf:
                    return gender == Gender.Male ? "HIM" : "HIF";
                case PlayableRaceId.Dark_Elf:
                    return gender == Gender.Male ? "DAM" : "DAF";
                case PlayableRaceId.Half_Elf:
                    return gender == Gender.Male ? "HAM" : "HAF";
                case PlayableRaceId.Dwarf:
                    return gender == Gender.Male ? "DWM" : "DWF";
                case PlayableRaceId.Troll:
                    return gender == Gender.Male ? "TRM" : "TRF";
                case PlayableRaceId.Ogre:
                    return gender == Gender.Male ? "OGM" : "OGF";
                case PlayableRaceId.Halfling:
                    return gender == Gender.Male ? "HOM" : "HOF";
                case PlayableRaceId.Gnome:
                    return gender == Gender.Male ? "GNM" : "GNF";
                case PlayableRaceId.Iksar:
                    return gender == Gender.Male ? "IKM" : "IKF";
            }

            return string.Empty;
        }

        public static List<int> PlayableRaceIds = new List<int>
        {
            1,
            2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 128
        };

        public static RaceIdDb GetRaceIdDb(int raceId)
        {
            if (raceId == 128)
            {
                return RaceIdDb.Iksar;
            }
            
            return (RaceIdDb)MathHelper.GetPowerOfTwo(raceId - 1);
        }
    }
}