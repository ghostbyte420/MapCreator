using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapCreator.Controls.UserSubmittedUtilities.TerrainGenerator
{
    public class Terrain
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int TileID { get; set; }
        public Color Color { get; set; }
        public int Base { get; set; }
        public bool Random { get; set; }

        public Terrain(string name, int id, int tileID, int r, int g, int b, int baseValue, bool random)
        {
            Name = name;
            ID = id;
            TileID = tileID;
            Color = Color.FromArgb(r, g, b);
            Base = baseValue;
            Random = random;
        }
    }
}
