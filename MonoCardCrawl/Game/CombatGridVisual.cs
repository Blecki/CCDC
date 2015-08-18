using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gem;

namespace Game
{
    public class CombatGridVisual : Gem.Render.ISceneNode, IInteractive
    {
        internal bool MouseHover = false;
        internal CombatCell CellUnderMouse = null;
        internal CombatGrid Grid;

        public Texture2D HoverTexture;
        public Texture2D[] TextureTable;

        public CombatGridVisual(CombatGrid Grid, Euler Euler = null)
        {
            this.Grid = Grid;
            this.Orientation = Euler;
            if (this.Orientation == null) this.Orientation = new Euler();
        }

        public override void CalculateLocalMouse(Ray MouseRay, Action<Gem.Render.ISceneNode, float> HoverCallback)
        {
            MouseHover = false;
            CellUnderMouse = null;

            var closestIntersection = Grid.RayIntersection(MouseRay);
            if (closestIntersection.Intersects)
            {
                //MouseHover = true;
                CellUnderMouse = closestIntersection.Tag as CombatCell;
                HoverCallback(this, closestIntersection.Distance);
            }
        }

        public override void HandleMouseHover()
        {
            MouseHover = true;
        }

        public PlayerAction GetClickAction()
        {
            if (CellUnderMouse != null) return CellUnderMouse.ClickAction;
            return null;
        }

        public override void Draw(Gem.Render.RenderContext Context)
        {
            Context.Color = new Vector3(1, 1, 1);
            Context.World = Orientation.Transform;
            Context.NormalMap = Context.NeutralNormals;
            Context.LightingEnabled = false;

            Grid.Cells.forAll((c, x, y, z) =>
                {
                    if (c != null && c.Visible)
                    {
                        Context.Texture = TextureTable[c.Texture];
                        Context.ApplyChanges();
                        Context.Draw(c.Mesh);

                        if (HoverTexture != null && MouseHover && Object.ReferenceEquals(CellUnderMouse, c))
                        {
                            Context.Texture = HoverTexture;
                            Context.ApplyChanges();
                            Context.Draw(c.Mesh);
                        }
                    }
                });


        }
    }
}
