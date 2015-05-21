using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Gem.Render
{
    public class RenderContext
    {
		protected Effect Effect;
        protected GraphicsDevice device;
        public ICamera Camera;
		protected int Technique = 0;
		public bool ProtectDiffuseColor = false;

        public Texture2D White { get; private set; }
		private VertexPositionNormalTexture[] VertexBuffer = new VertexPositionNormalTexture[8];

        public void BeginScene(Effect effect, int Technique, GraphicsDevice device)
        {
			this.Effect = effect;
			this.Technique = Technique;
            this.device = device;

            if (White == null)
            {
                White = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
                White.SetData(new Color[] { new Color(255, 255, 255, 255) });
            }

            ApplyChanges();
        }

        public void ApplyChanges()
        {
			Effect.CurrentTechnique = Effect.Techniques[Technique];
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        public Matrix World
        {
            set
            {
				Effect.Parameters["World"].SetValue(value);
				Effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(value)));
            }
        }

        public virtual Texture2D Texture
        {
            set
            {
			    Effect.Parameters["Texture"].SetValue(value);
            }
        }

        public virtual Vector3 Color
        {
            set
            {
				if (!ProtectDiffuseColor)
					Effect.Parameters["DiffuseColor"].SetValue(new Vector4(value, 1.0f));
            }
        }

        public void Draw(Geo.CompiledModel model)
        {
            device.SetVertexBuffer(model.verticies);
            device.Indices = model.indicies;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, model.verticies.VertexCount,
                0, System.Math.Min(model.primitiveCount, 65535));
        }

        public void Draw(Geo.Mesh mesh)
        {
            if (mesh.verticies.Length > 0)
            {
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, mesh.verticies, 0, mesh.verticies.Length,
                    mesh.indicies, 0, mesh.indicies.Length / 3);
            }
        }

		public void DrawLine(Vector3 v0, Vector3 v1, float width, Vector3 n)
		{
			var tangent = v1 - v0;
			tangent.Normalize();
			var offset = Vector3.Transform(tangent, Matrix.CreateFromAxisAngle(n, (float)(System.Math.PI / 2)));
			offset.Normalize();
			offset *= width / 2;

			VertexBuffer[0].Position = v0 + offset;
			VertexBuffer[1].Position = v1 + offset;
			VertexBuffer[2].Position = v0 - offset;
			VertexBuffer[3].Position = v1 + offset;
			VertexBuffer[4].Position = v1 - offset;
			VertexBuffer[5].Position = v0 - offset;

			for (int i = 0; i < 6; ++i) VertexBuffer[i].Normal = n;

			device.DrawUserPrimitives(PrimitiveType.TriangleList, VertexBuffer, 0, 2);
		}

		public void DrawPoint()
		{
			VertexBuffer[0].Position = new Vector3(-0.5f, -0.5f, 0);
			VertexBuffer[1].Position = new Vector3(-0.5f, 0.5f, 0);
			VertexBuffer[2].Position = new Vector3(0.5f, -0.5f, 0);
			VertexBuffer[3].Position = new Vector3(0.5f, 0.5f, 0);
			for (int i = 0; i < 6; ++i) VertexBuffer[i].Normal = Vector3.UnitZ;

			device.DrawUserPrimitives(PrimitiveType.TriangleStrip, VertexBuffer, 0, 2);
		}

        public void DrawSprite(Geo.Mesh mesh)
        {
            if (mesh.verticies.Length > 0)
            {
                //spriteEffect.CurrentTechnique.Passes[0].Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, mesh.verticies, 0, mesh.verticies.Length,
                    mesh.indicies, 0, mesh.indicies.Length / 3);
            }
        }

		public void DrawLineIM(Vector3 v0, Vector3 v1)
		{
			VertexBuffer[0].Position = v0;
			VertexBuffer[1].Position = v1;
			device.DrawUserPrimitives(PrimitiveType.LineList, VertexBuffer, 0, 1);
		}

        public void DrawLines(Geo.Mesh mesh)
        {
            if (mesh.lineIndicies != null && mesh.verticies.Length > 0)
                device.DrawUserIndexedPrimitives(PrimitiveType.LineList, mesh.verticies, 0, mesh.verticies.Length,
                    mesh.lineIndicies, 0, mesh.lineIndicies.Length / 2);
        }


		public void DrawHitbox(float X, float Y, float W, float H)
		{

			VertexBuffer[0].Position = new Vector3(X, Y, 0);
			VertexBuffer[1].Position = new Vector3(X, Y + H, 0);
			VertexBuffer[2].Position = new Vector3(X + W, Y, 0);
			VertexBuffer[3].Position = new Vector3(X + W, Y + H, 0);
			for (int i = 0; i < 4; ++i) VertexBuffer[i].Normal = Vector3.UnitZ;

			device.DrawUserPrimitives(PrimitiveType.TriangleStrip, VertexBuffer, 0, 2);
		}
	}

}
