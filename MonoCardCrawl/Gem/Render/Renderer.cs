using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Gem;

namespace Gem.Render
{
    public class Renderer
    {
        public ICamera Camera = new Cameras.OrbitCamera(Vector3.Zero, Vector3.UnitX, Vector3.UnitZ, 10);
		Effect Effect;
        ImmediateModeDebug debug;
        GraphicsDevice device;

        RenderContext renderContext = new RenderContext();
        ImmediateMode2d immediate2d = null;

        List<Tuple<VertexPositionColor, VertexPositionColor>> debugLines = 
            new List<Tuple<VertexPositionColor, VertexPositionColor>>();

        public void AddDebugLine(VertexPositionColor a, VertexPositionColor b)
        {
            debugLines.Add(new Tuple<VertexPositionColor, VertexPositionColor>(a, b));
        }

        public Renderer(GraphicsDevice device, ContentManager content)
        {
            this.device = device;
            debug = new ImmediateModeDebug(device);

			Effect = content.Load<Effect>("draw");

            Camera.Viewport = device.Viewport;


            immediate2d = new ImmediateMode2d(device);
        }

        public void PreDraw(float elapsedSeconds, IRenderable renderable)
        {
			if (renderable != null)
			{
				renderContext.Camera = Camera;
				renderable.PreDraw(elapsedSeconds, device, renderContext);
			}
        }

        public Ray GetMouseRay(Vector2 mouseCoordinates)
        {
            var mouseRay = new Ray(
                Camera.GetPosition(), 
                Camera.Unproject(new Vector3(mouseCoordinates, 0)) - Camera.GetPosition());
            mouseRay.Direction = Vector3.Normalize(mouseRay.Direction);
            return mouseRay;
        }

        public enum DrawModeFlag
        {
            Normal,
            DebugOnly
        }

        public void Draw(IEnumerable<IRenderable> renderables, DrawModeFlag modeFlag = DrawModeFlag.Normal)
        {
            device.Clear(ClearOptions.Target, Color.CornflowerBlue, 0xFFFFFF, 0);           

			Effect.Parameters["World"].SetValue(Matrix.Identity);
			Effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.Identity)));
			Effect.Parameters["View"].SetValue(Camera.View);
			Effect.Parameters["Projection"].SetValue(Camera.Projection);
			
            renderContext.BeginScene(Effect, 0, device);

            renderContext.Camera = Camera;
            renderContext.Color = Vector3.One;

            if (modeFlag == DrawModeFlag.Normal)
                foreach (var node in renderables)
                    if (node != null) node.DrawEx(renderContext, RenderMode.Normal);

            debug.Begin(Matrix.Identity, Camera.View, Camera.Projection);
            foreach (var line in debugLines)
                debug.Line(line.Item1, line.Item2);
            debug.Flush();
            debugLines.Clear();
        }

        public RenderContext GetContext()
        {
            return renderContext;
        }
    }
}
