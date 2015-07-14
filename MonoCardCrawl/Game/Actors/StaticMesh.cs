using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MonoCardCrawl.Game.Actors
{
    public class StaticMesh : Actor
    {
        public StaticMesh(Gem.Geo.Mesh Mesh, Texture2D Texture)
        {
            Renderable = new Gem.Render.SceneGraph.MeshNode(Mesh, Texture, Orientation);
        }
    }
}
