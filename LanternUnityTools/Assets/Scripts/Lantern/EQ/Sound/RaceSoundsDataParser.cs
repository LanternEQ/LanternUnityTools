using System;
using System.Collections.Generic;
using System.IO;
using Lantern.EQ;
using UnityEngine;

namespace Lantern
{
    public class RaceSoundsDataParser : MonoBehaviour
    {
        public static List<ModelSound> GetModelSounds()
        {
            var modelSoundsAssetPath = Application.streamingAssetsPath + "/ClientData/RaceSounds.csv";
            var textLines = File.ReadAllLines(modelSoundsAssetPath);
            List<ModelSound> modelSounds = new List<ModelSound>();

            for (var i = 0; i < textLines.Length; i++)
            {
                // Skip the header
                if (i < 2)
                {
                    continue;
                }
            
                var line = textLines[i];

                var elements = line.Split(',');
            
                ModelSound newSound = new ModelSound();

                newSound.RaceId = Convert.ToInt32(elements[0]);
                newSound.VariantId = Convert.ToInt32(elements[1]);

                switch (elements[2])
                {
                    case "M":
                        newSound.GenderId = Gender.Male;
                        break;
                    case "F":
                        newSound.GenderId = Gender.Female;
                        break;
                    case "N":
                        newSound.GenderId = Gender.Neutral;
                        break;
                }
            
                newSound.Loop = elements[3];
                newSound.Idle1 = elements[4];
                newSound.Idle2 = elements[5];
                newSound.Jump = elements[6];
                newSound.GetHit1 = elements[7];
                newSound.GetHit2 = elements[8];
                newSound.GetHit3 = elements[9];
                newSound.GetHit4 = elements[10];
                newSound.Gasp1 = elements[11];
                newSound.Gasp2 = elements[12];
                newSound.Death = elements[13];
                newSound.Drown = elements[14];
                newSound.Walking = elements[15];
                newSound.Running = elements[16];
                newSound.Attack = elements[17];
                newSound.SAttack = elements[18];
                newSound.TAttack = elements[19];

                modelSounds.Add(newSound);
            }

            return modelSounds;
        }
    }
}
