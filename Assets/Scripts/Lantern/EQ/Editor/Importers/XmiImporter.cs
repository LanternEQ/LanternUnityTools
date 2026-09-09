using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using Lantern.EQ.Audio;
using Lantern.EQ.Audio.Xmi;

namespace Lantern.EQ.Editor.Importers
{
    [ScriptedImporter(version: 1, ext: "xmi", AllowCaching = true)]
    public class XmiImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var fileName = Path.GetFileNameWithoutExtension(ctx.assetPath);
            var midiTrackCollection = ScriptableObject.CreateInstance<MidiTrackCollection>();

            ctx.AddObjectToAsset($"{fileName}.miditracks", midiTrackCollection);
            ctx.SetMainObject(midiTrackCollection);

            var xmiStream = File.OpenRead(ctx.assetPath);
            var xmiReader = new XmiFileReader(xmiStream);

            var xmiFile = xmiReader.ReadXmiFile();
            var trackCount = xmiFile.XmidiTracks.Length;

            for (int trackNumber = 0; trackNumber < trackCount; trackNumber++)
            {
                var midiStream = xmiFile.WriteMidiTrack(trackNumber);
                if (midiStream is MemoryStream memoryStream)
                {
                    var midiTrack = MidiTrack.CreateMidiTrack(fileName, trackNumber, memoryStream.ToArray());
                    ctx.AddObjectToAsset($"{fileName}_{trackNumber}.miditrack", midiTrack);
                    midiTrackCollection.MidiTracks.Add(midiTrack);
                }
                else
                {
                    ctx.LogImportError($"[XmiImporter] Failed to create midi for track {trackNumber}");
                }
            }
        }
    }
}
