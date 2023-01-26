using System.Collections.Generic;
using Lantern.EQ.Data;

// TODO: Move this elsewhere

namespace Lantern.EQ.Helpers
{
    public static class RaceHelper
    {
        public static bool IsPlayableRaceId(int raceId)
        {
            return raceId < 13 || raceId == 70 || raceId == 128 || raceId == 130;
        }

        public static bool IsPlayableRaceModel(string model)
        {
            switch (model.ToUpper())
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

        public static bool IsBoat(int raceId)
        {
            switch (raceId)
            {
                case 72:  // Ship
                case 73:  // Launch
                case 114: // Ghost Ship
                case 141: // Boat
                    return true;
                default:
                    return false;
            }
        }

        public static GenderId GetGenderFromGenderId(int genderId)
        {
            switch (genderId)
            {
                case 0:
                    return GenderId.Male;
                case 1:
                    return GenderId.Female;
                case 2:
                    return GenderId.Neutral;
            }

            return GenderId.Male;
        }

        public static string GetDefaultModelForGender(GenderId genderId)
        {
            switch (genderId)
            {
                case GenderId.Female:
                    return EqConstants.DefaultModelFemale;
                default:
                    return EqConstants.DefaultModelMale;
            }
        }

        public static string GetDefaultModelForGender(int genderId)
        {
            return GetDefaultModelForGender((GenderId)genderId);
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

        public static float GetCapsuleWidthForRace(int raceId)
        {
            // TODO: Need to flesh this data out. Tried looking for it in the client. No luck so far. Will have to sampled
            return 0.1875f;
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

        public static readonly List<int> PlayableRaceIds = new List<int>
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 128
        };

        public static RaceIdDb GetRaceIdDb(int raceId)
        {
            if (raceId == 128)
            {
                return RaceIdDb.Iksar;
            }

            return (RaceIdDb)MathHelper.GetPowerOfTwo(raceId - 1);
        }

        public static bool IsKoboldVariantEdgeCase(int texture, int helmTexture, int raceId)
        {
            return raceId == 48 && texture == 2 || raceId == 48 && texture == 1 && helmTexture == 0;
        }

        public static bool IsHiddenNameRace(int raceId)
        {
            switch (raceId)
            {
                // gargoyle, mimic, skeleton
                case 29:
                case 52:
                // case 60: and an unknown flag is set
                    return true;
            }
            return false;
        }

        public static bool IsPlayableRace(int raceId)
        {
            return PlayableRaceIds.Contains(raceId);
        }
    }
}
