using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gem.Geo;
using Gem;

namespace Gem.Render
{
    public class NormalMapMeshNode : ISceneNode
    {
        public Mesh Mesh;
        public Vector3 Color = Vector3.One;
        public Texture2D Texture = null;
        public Texture2D NormalMap = null;
        public Matrix UVTransform = Matrix.Identity;

        public NormalMapMeshNode(Mesh Mesh, Texture2D Texture, Texture2D NormalMap, Euler Orientation = null) 
        { 
            this.Mesh = Mesh;
            this.Texture = Texture;
            this.NormalMap = NormalMap;
            this.Orientation = Orientation;
            if (this.Orientation == null) this.Orientation = new Euler();
        }

        public override void Draw(Gem.Render.RenderContext context)
        {
            context.Color = Color;
            if (Texture != null) context.Texture = Texture;
            context.NormalMap = NormalMap;
            context.World = WorldTransform;
            context.UVTransform = UVTransform;
            context.ApplyChanges();
            context.Draw(Mesh);
            context.NormalMap = context.NeutralNormals;
            context.UVTransform = Matrix.Identity;
        }

    }
}
