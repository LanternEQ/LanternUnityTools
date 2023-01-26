using System;
using System.Collections.Generic;
using System.IO;
using Infrastructure.EQ.TextParser;
using UnityEngine;

namespace Lantern.EQ.Helpers
{
    public class RaceDataParser
    {
        private static Dictionary<int, RaceData> RaceValues = new Dictionary<int, RaceData>();

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
    }
}
