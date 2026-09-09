using Infrastructure.Lantern.SQLite;

namespace Lantern.EQ.Data
{
    /// <summary>
    /// A row of the doors table, mapping the columns the door importer places into a zone
    /// </summary>
    public class Doors
    {
        [PrimaryKey] public int id { get; set; }
        public int doorid { get; set; }

        public string zone { get; set; }

        public float pos_x { get; set; }
        public float pos_y { get; set; }
        public float pos_z { get; set; }
        public string name { get; set; }
        public int opentype { get; set; }
        public float heading { get; set; }
    }
}
