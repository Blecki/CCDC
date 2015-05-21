using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoCardCrawl
{
    public class Cell
    {
        public Tile Tile;
        public String LoadedTileName = "";

        public bool Solid
        {
            get
            {
                if (Tile == null) return false;
                return true;
            }
        }

    }
}
