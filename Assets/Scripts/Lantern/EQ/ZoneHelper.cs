using System.Collections;
using System.Collections.Generic;

public static class ZoneHelper
{
    public class ZoneDesc
    {
        public string shortname;
        public string longname;
        public string continent;

        public ZoneDesc(string ShortName, string LongName, string Continent)
        {
            shortname = ShortName;
            longname = LongName;
            continent = Continent;
        }
    }

    private static Dictionary<string, ZoneDesc> shortnameDict = new Dictionary<string, ZoneDesc>
    {
        { "airplane", new ZoneDesc("airplane", "Plane of Sky", "Planes") },
        { "akanon", new ZoneDesc("akanon", "Ak'Anon", "Faydwer") },
        { "arena", new ZoneDesc("arena", "The Arena", "Antonica") },
        { "befallen", new ZoneDesc("befallen", "Befallen", "Antonica") },
        { "beholder", new ZoneDesc("beholder", "Gorge of King Xorbb", "Antonica") },
        { "blackburrow", new ZoneDesc("blackburrow", "Blackburrow", "Antonica") },
        { "burningwood", new ZoneDesc("burningwood", "The Burning Wood", "Kunark") },
        { "butcher", new ZoneDesc("butcher", "Butcherblock Mountains", "Faydwer") },
        { "cabeast", new ZoneDesc("cabeast", "Cabilis East", "Kunark") },
        { "cabwest", new ZoneDesc("cabwest", "Cabilis West", "Kunark") },
        { "cauldron", new ZoneDesc("cauldron", "Dagnor's Cauldron", "Faydwer") },
        { "cazicthule", new ZoneDesc("cazicthule", "Lost Temple of CazicThule", "Antonica") },
        { "charasis", new ZoneDesc("charasis", "The Howling Stones", "Kunark") },
        { "chardok", new ZoneDesc("chardok", "Chardok", "Kunark") },
        { "citymist", new ZoneDesc("citymist", "The City of Mist", "Kunark") },
        { "cobaltscar", new ZoneDesc("cobaltscar", "Cobalt Scar", "Velious") },
        { "commons", new ZoneDesc("commons", "West Commonlands", "Antonica") },
        { "crushbone", new ZoneDesc("crushbone", "Crushbone", "Faydwer") },
        { "crystal", new ZoneDesc("crystal", "Crystal Caverns", "Velious") },
        { "cshome", new ZoneDesc("cshome", "EQClassic Guide Zone", "Other") },
        { "dalnir", new ZoneDesc("dalnir", "Dalnir", "Kunark") },
        { "dreadlands", new ZoneDesc("dreadlands", "Dreadlands", "Kunark") },
        { "droga", new ZoneDesc("droga", "Mines of Droga", "Kunark") },
        { "eastkarana", new ZoneDesc("eastkarana", "Eastern Plains of Karana", "Antonica") },
        { "eastwastes", new ZoneDesc("eastwastes", "Eastern Wastelands", "Velious") },
        { "ecommons", new ZoneDesc("ecommons", "East Commonlands", "Antonica") },
        { "emeraldjungle", new ZoneDesc("emeraldjungle", "The Emerald Jungle", "Kunark") },
        { "erudnext", new ZoneDesc("erudnext", "Erudin", "Odus") },
        { "erudnint", new ZoneDesc("erudnint", "Erudin Palace", "Odus") },
        { "erudsxing", new ZoneDesc("erudsxing", "Erud's Crossing", "Odus") },
        { "everfrost", new ZoneDesc("everfrost", "Everfrost", "Antonica") },
        { "fearplane", new ZoneDesc("fearplane", "Plane of Fear", "Planes") },
        { "feerrott", new ZoneDesc("feerrott", "The Feerrott", "Antonica") },
        { "felwithea", new ZoneDesc("felwithea", "Northern Felwithe", "Faydwer") },
        { "felwitheb", new ZoneDesc("felwitheb", "Southern Felwithe", "Faydwer") },
        { "fieldofbone", new ZoneDesc("fieldofbone", "Field of Bone", "Kunark") },
        { "firiona", new ZoneDesc("firiona", "Firiona Vie", "Kunark") },
        { "freporte", new ZoneDesc("freporte", "East Freeport", "Antonica") },
        { "freportn", new ZoneDesc("freportn", "North Freeport", "Antonica") },
        { "freportw", new ZoneDesc("freportw", "West Freeport", "Antonica") },
        { "frontiermtns", new ZoneDesc("frontiermtns", "Frontier Mountains", "Kunark") },
        { "frozenshadow", new ZoneDesc("frozenshadow", "Tower of Frozen Shadow", "Velious") },
        { "gfaydark", new ZoneDesc("gfaydark", "Greater Faydark", "Faydwer") },
        { "greatdivide", new ZoneDesc("greatdivide", "Great Divide", "Velious") },
        { "grobb", new ZoneDesc("grobb", "Grobb", "Antonica") },
        { "growthplane", new ZoneDesc("growthplane", "Plane of Growth", "Planes") },
        { "gukbottom", new ZoneDesc("gukbottom", "Ruins of Old Guk", "Antonica") },
        { "guktop", new ZoneDesc("guktop", "Guk", "Antonica") },
        { "halas", new ZoneDesc("halas", "Halas", "Antonica") },
        { "hateplane", new ZoneDesc("hateplane", "Plane of Hate", "Planes") },
        { "highkeep", new ZoneDesc("highkeep", "High Keep", "Antonica") },
        { "highpass", new ZoneDesc("highpass", "Highpass Hold", "Antonica") },
        { "hole", new ZoneDesc("hole", "The Hole", "Odus") },
        { "iceclad", new ZoneDesc("iceclad", "Iceclad Ocean", "Velious") },
        { "innothule", new ZoneDesc("innothule", "Innothule Swamp", "Antonica") },
        { "kael", new ZoneDesc("kael", "Kael Drakael", "Velious") },
        { "kaesora", new ZoneDesc("kaesora", "Kaesora", "Kunark") },
        { "kaladima", new ZoneDesc("kaladima", "North Kaladim", "Faydwer") },
        { "kaladimb", new ZoneDesc("kaladimb", "South Kaladim", "Faydwer") },
        { "karnor", new ZoneDesc("karnor", "Karnor's Castle", "Kunark") },
        { "kedge", new ZoneDesc("kedge", "Kedge Keep", "Faydwer") },
        { "kerraridge", new ZoneDesc("kerraridge", "Kerra Isle", "Odus") },
        { "kithicor", new ZoneDesc("kithicor", "Kithicor Woods", "Antonica") },
        { "kurn", new ZoneDesc("kurn", "Kurn's Tower", "Kunark") },
        { "lakeofillomen", new ZoneDesc("lakeofillomen", "Lake of Ill Omen", "Kunark") },
        { "lakerathe", new ZoneDesc("lakerathe", "Lake Rathetear", "Antonica") },
        { "lavastorm", new ZoneDesc("lavastorm", "Lavastorm Mountains", "Antonica") },
        { "lfaydark", new ZoneDesc("lfaydark", "Lesser Faydark", "Faydwer") },
        { "load", new ZoneDesc("load", "Loading Zone", "Other") },
        { "mischiefplane", new ZoneDesc("mischiefplane", "Plane of Mischief", "Planes") },
        { "mistmoore", new ZoneDesc("mistmoore", "Castle Mistmoore", "Faydwer") },
        { "misty", new ZoneDesc("misty", "Misty Thicket", "Antonica") },
        { "najena", new ZoneDesc("najena", "Najena", "Antonica") },
        { "necropolis", new ZoneDesc("necropolis", "Dragon Necropolis", "Velious") },
        { "nektulos", new ZoneDesc("nektulos", "Nektulos Forest", "Antonica") },
        { "neriaka", new ZoneDesc("neriaka", "Neriak Foreign Quarter", "Antonica") },
        { "neriakb", new ZoneDesc("neriakb", "Neriak Commons", "Antonica") },
        { "neriakc", new ZoneDesc("neriakc", "Neriak Third Gate", "Antonica") },
        { "northkarana", new ZoneDesc("northkarana", "Northern Plains of Karana", "Antonica") },
        { "nro", new ZoneDesc("nro", "Northern Desert of Ro", "Antonica") },
        { "nurga", new ZoneDesc("nurga", "Mines of Nurga", "Kunark") },
        { "oasis", new ZoneDesc("oasis", "Oasis of Marr", "Antonica") },
        { "oggok", new ZoneDesc("oggok", "Oggok", "Antonica") },
        { "oot", new ZoneDesc("oot", "Ocean of Tears", "Antonica") },
        { "overthere", new ZoneDesc("overthere", "The Overthere", "Kunark") },
        { "paineel", new ZoneDesc("paineel", "Paineel", "Odus") },
        { "paw", new ZoneDesc("paw", "Lair of the Splitpaw", "Antonica") },
        { "permafrost", new ZoneDesc("permafrost", "Permafrost Caverns", "Antonica") },
        { "qcat", new ZoneDesc("qcat", "Qeynos Aqueduct System", "Antonica") },
        { "qey2hh1", new ZoneDesc("qey2hh1", "Western Plains of Karana", "Antonica") },
        { "qeynos", new ZoneDesc("qeynos", "South Qeynos", "Antonica") },
        { "qeynos2", new ZoneDesc("qeynos2", "North Qeynos", "Antonica") },
        { "qeytoqrg", new ZoneDesc("qeytoqrg", "Qeynos Hills", "Antonica") },
        { "qrg", new ZoneDesc("qrg", "Surefall Glade", "Antonica") },
        { "rujarkian", new ZoneDesc("rujarkian", "Rujarkian Hills", "Custom") },
        { "rathemtn", new ZoneDesc("rathemtn", "Rathe Mountains", "Antonica") },
        { "rivervale", new ZoneDesc("rivervale", "Rivervale", "Antonica") },
        { "runnyeye", new ZoneDesc("runnyeye", "Runnyeye Citadel", "Antonica") },
        { "sebilis", new ZoneDesc("sebilis", "Old Sebilis", "Kunark") },
        { "sirens", new ZoneDesc("sirens", "Sirens Grotto", "Velious") },
        { "skyfire", new ZoneDesc("skyfire", "Skyfire Mountains", "Kunark") },
        { "skyshrine", new ZoneDesc("skyshrine", "Skyshrine", "Velious") },
        { "sleeper", new ZoneDesc("sleeper", "Sleepers Tomb", "Velious") },
        { "soldunga", new ZoneDesc("soldunga", "Solusek's Eye", "Antonica") },
        { "soldungb", new ZoneDesc("soldungb", "Nagafen's Lair", "Antonica") },
        { "soltemple", new ZoneDesc("soltemple", "Temple of Solusek Ro", "Antonica") },
        { "southkarana", new ZoneDesc("southkarana", "Southern Plains of Karana", "Antonica") },
        { "sro", new ZoneDesc("sro", "Southern Desert of Ro", "Antonica") },
        { "steamfont", new ZoneDesc("steamfont", "Steamfont Mountains", "Faydwer") },
        { "stonebrunt", new ZoneDesc("stonebrunt", "Stonebrunt Mountains", "Odus") },
        { "swampofnohope", new ZoneDesc("swampofnohope", "Swamp Of No Hope", "Kunark") },
        { "templeveeshan", new ZoneDesc("templeveeshan", "Temple of Veeshan", "Velious") },
        { "thurgadina", new ZoneDesc("thurgadina", "City of Thurgadin", "Velious") },
        { "thurgadinb", new ZoneDesc("thurgadinb", "Icewell Keep", "Velious") },
        { "timorous", new ZoneDesc("timorous", "Timorous Deep", "Kunark") },
        { "tox", new ZoneDesc("tox", "Toxxulia Forest", "Odus") },
        { "trakanon", new ZoneDesc("trakanon", "Trakanon's Teeth", "Kunark") },
        { "tutorial", new ZoneDesc("tutorial", "The Tutorial Zone", "Other") },
        { "unrest", new ZoneDesc("unrest", "Estate of Unrest", "Faydwer") },
        { "veeshan", new ZoneDesc("veeshan", "Veeshan's Peak", "Kunark") },
        { "velketor", new ZoneDesc("velketor", "Velketor's Labrynth", "Velious") },
        { "wakening", new ZoneDesc("wakening", "The Wakening Lands", "Velious") },
        { "warrens", new ZoneDesc("warrens", "The Warrens", "Odus") },
        { "warslikswood", new ZoneDesc("warslikswood", "Warslilks Woods", "Kunark") },
        { "westwastes", new ZoneDesc("westwastes", "Western Wastelands", "Velious") },
        { "construct", new ZoneDesc("construct", "The Construct", "Other") }
    };

    public static Dictionary<string, ZoneDesc> ShortnameDict => shortnameDict;

    public static bool IsValidZoneShortname(string shortname)
    {
        return shortnameDict.ContainsKey(shortname);
    }
}