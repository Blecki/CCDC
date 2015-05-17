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
        //BasicEffect drawEffect;
		Effect Effect;
        //BasicEffect drawIDEffect;
        //AlphaTestEffect drawSpriteEffect;
        ImmediateModeDebug debug;
        GraphicsDevice device;

        RenderTarget2D mousePickTarget;
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
			//Effect.Parameters["PointLight1Position"].SetValue(new Vector4(3,3,3,1));
			//Effect.Parameters["PointLight1Color"].SetValue(new Vector4(1, 1, 1, 1));
			//Effect.Parameters["AttenuationMap"].SetValue(content.Load<Texture2D>("attenuation"));

            Camera.Viewport = device.Viewport;

            mousePickTarget = new RenderTarget2D(device, 1, 1, false, SurfaceFormat.Color, DepthFormat.Depth24);

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

        public IRenderable MousePick(Vector2 mouseCoordinates, IEnumerable<IRenderable> renderables)
        {
            device.SetRenderTarget(mousePickTarget);
            device.Clear(ClearOptions.Target, Vector4.Zero, 0xFFFFFF, 0);
            device.BlendState = BlendState.Opaque;

			Effect.Parameters["World"].SetValue(Matrix.Identity);
			Effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.Identity)));
			Effect.Parameters["View"].SetValue(Camera.View);
			Effect.Parameters["Projection"].SetValue(Camera.GetSinglePixelProjection(mouseCoordinates));

            renderContext.BeginScene(Effect, 1, device);
			renderContext.ProtectDiffuseColor = true;

            var index = 1;
            foreach (var renderable in renderables)
            {
				if (renderable != null)
				{
					var idBytes = BitConverter.GetBytes(index);
					Effect.Parameters["DiffuseColor"].SetValue(
						new Vector4(idBytes[0] / 255.0f, idBytes[1] / 255.0f, idBytes[2] / 255.0f, 1.0f));

					renderable.DrawEx(renderContext, RenderMode.MousePick);
				}
                index += 1;
            }

			renderContext.ProtectDiffuseColor = false;
            device.SetRenderTarget(null);
            var data = new Color[1];
            mousePickTarget.GetData(data);
            var result = data[0].PackedValue & 0x00FFFFFF; //Mask off the alpha bits.

			if (result != 0)
			{
				try
				{
					return renderables.ElementAt((int)(result - 1));
				}
				catch (Exception e)
				{ }
			}

            return null;
        }

        public enum DrawModeFlag
        {
            Normal,
            DebugOnly
        }

        public void Draw(IEnumerable<IRenderable> renderables, DrawModeFlag modeFlag = DrawModeFlag.Normal)
        {
			device.RasterizerState = RasterizerState.CullCounterClockwise;
            //device.SetRenderTarget(null);
            device.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.Default;
			device.SamplerStates[0] = SamplerState.PointClamp;

			Effect.Parameters["World"].SetValue(Matrix.Identity);
			Effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.Identity)));
			Effect.Parameters["View"].SetValue(Camera.View);
			Effect.Parameters["Projection"].SetValue(Camera.Projection);
			//Effect.Parameters["PointLight1Position"].SetValue(new Vector4(3, 3, 3, 1));
			//Effect.Parameters["PointLight1Color"].SetValue(new Vector4(1, 1, 1, 1));

            renderContext.Camera = Camera;
            renderContext.BeginScene(Effect, 0, device);

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
