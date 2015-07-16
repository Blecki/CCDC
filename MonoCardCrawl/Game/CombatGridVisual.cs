using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gem;

namespace MonoCardCrawl
{
    public class CombatGridVisual : Gem.Render.SceneGraph.ISceneNode
    {
        internal bool MouseHover = false;
        internal CombatCell CellUnderMouse = null;
        internal CombatGrid Grid;
        internal Gem.Geo.Mesh LinkVisualizationQuad;

        public CombatGridVisual(CombatGrid Grid, Euler Euler = null)
        {
            this.Grid = Grid;
            this.Orientation = Euler;
            if (this.Orientation == null) this.Orientation = new Euler();

            LinkVisualizationQuad = Gem.Geo.Gen.FacetCopy(Gem.Geo.Gen.CreateQuad());
            Gem.Geo.Gen.Transform(LinkVisualizationQuad, Matrix.CreateScale(0.25f));
        }

        public override void CalculateLocalMouse(Ray MouseRay)
        {
            MouseHover = false;
            CellUnderMouse = null;

            var closestIntersection = Grid.RayIntersection(MouseRay);
            if (closestIntersection.Intersects)
            {
                MouseHover = true;
                CellUnderMouse = closestIntersection.Tag as CombatCell;
            }
        }

        public override void Draw(Gem.Render.RenderContext Context)
        {
            Context.Color = new Vector3(1, 0, 0);
            Context.World = Orientation.Transform;
            Context.NormalMap = Context.NeutralNormals;
            Context.LightingEnabled = false;

            Grid.Cells.forAll((c, x, y, z) =>
                {
                    if (c != null && c.Visible)
                    {
                        if (Object.ReferenceEquals(CellUnderMouse, c)) Context.Color = new Vector3(0, 1, 1);
                        else Context.Color = Vector3.One;

                        Context.World = Orientation.Transform;

                        Context.Texture = c.Texture;
                        Context.ApplyChanges();
                        Context.Draw(c.Mesh);

                        //foreach (var link in c.Links)
                        //{
                        //    Context.Texture = Context.White;
                        //    Context.World = Matrix.CreateTranslation(CombatCell.DirectionOffset(link.Direction) * 0.4f) * Matrix.CreateTranslation(x + 0.5f,y + 0.5f,z + 0.3f) * Orientation.Transform;
                        //    Context.ApplyChanges();
                        //    Context.Draw(LinkVisualizationQuad);
                        //}
                    }
                });
        }
    }
}
