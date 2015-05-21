using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gem.Render;
using Gem;

namespace MonoCardCrawl
{
    public class TileGridNode : Gem.Render.SceneGraph.ISceneNode
    {
        public Euler Orientation = null;
        public CellGrid Grid;

        public TileGridNode(CellGrid Grid, Euler Orientation = null) 
        { 
            this.Grid = Grid;
            this.Orientation = Orientation;
            if (this.Orientation == null) this.Orientation = new Euler();
        }

        private Matrix worldTransformation = Matrix.Identity;

        public void UpdateWorldTransform(Matrix m)
        {
            worldTransformation = m * Orientation.Transform;
        }

        public virtual void Draw(RenderContext context)
        {
            context.Color = Vector3.One;
            Grid.forRect(0, 0, 0, Grid.width, Grid.height, Grid.depth, (t, x, y, z) =>
                {
                    if (t.Tile != null && t.Tile.RenderMesh != null)
                    {
                        context.World = worldTransformation * Matrix.CreateTranslation(x + 0.5f, y + 0.5f, z);
                        context.Texture = t.Tile.Texture;
                        context.ApplyChanges();
                        context.Draw(t.Tile.RenderMesh);
                    }
                });
        }

        public void Visit(Action<Gem.Render.SceneGraph.ISceneNode> callback) { callback(this); }
        public void CalculateLocalMouse(Ray mouseRay, Action<VertexPositionColor, VertexPositionColor> debug) { }

		
    }
}
