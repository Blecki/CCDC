using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gem.Render
{
    public class SceneNode
    {
        public bool InteractWithMouse = true;

        protected Matrix WorldTransform;
        public Euler Orientation { get; set; }

        public virtual void UpdateWorldTransform(Matrix M)
        {
            WorldTransform = M * Orientation.Transform;
        }

        public virtual void PreDraw(float ElapsedSeconds, RenderContext Context) { }
        public virtual void Draw(RenderContext Context) { }
        public virtual void CalculateLocalMouse(Ray MouseRay, Action<SceneNode, float> HoverCallback) { }

        public virtual Action HoverAction { get; set; }
        public virtual Action ClickAction { get; set; }

        public virtual Action GetHoverAction() { return HoverAction; }
        public virtual Action GetClickAction() { return ClickAction; }
    }
}
