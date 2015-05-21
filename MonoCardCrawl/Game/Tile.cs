using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoCardCrawl
{
    public class Tile
    {
        public String Name;
        public Gem.Geo.Mesh RenderMesh;
        public Gem.Geo.Mesh NavigationMesh;
        public Microsoft.Xna.Framework.Graphics.Texture2D Texture;
    }
}
