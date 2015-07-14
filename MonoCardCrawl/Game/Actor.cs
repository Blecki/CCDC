using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoCardCrawl
{
    public class Actor
    {
        public Gem.Euler Orientation = new Gem.Euler();
        public Gem.Render.SceneGraph.ISceneNode Renderable = null;

        public virtual void Update(World World) { }
    }
}
