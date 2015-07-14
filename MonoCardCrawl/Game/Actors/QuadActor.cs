using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoCardCrawl.Game.Actors
{
    public class QuadActor : Actor
    {
        private Gem.Euler ShadowOrientation = new Gem.Euler();

        public QuadActor(Texture2D Sprite, Texture2D NormalMap, Texture2D DropShadow)
        {
            var branch = new Gem.Render.SceneGraph.BranchNode();

            var Mesh = Gem.Geo.Gen.CreateQuad();
            Gem.Geo.Gen.Transform(Mesh, Matrix.CreateRotationZ((float)Math.PI));
            Gem.Geo.Gen.Transform(Mesh, Matrix.CreateTranslation(0, 0.5f, 0));
            Gem.Geo.Gen.Transform(Mesh, Matrix.CreateScale(1, 1.5f, 1));
            Gem.Geo.Gen.Transform(Mesh, Matrix.CreateRotationX((float)(Math.PI / 8) * 3));
            Mesh = Gem.Geo.Gen.FacetCopy(Mesh);
            Gem.Geo.Gen.CalculateTangentsAndBiNormals(Mesh);

            branch.Add(new MonoCardCrawl.NormalMapMeshNode(Mesh, Sprite, NormalMap, Orientation));

            var shadowMesh = Gem.Geo.Gen.CreateQuad();
            Gem.Geo.Gen.Transform(shadowMesh, Matrix.CreateTranslation(0, 0, 0.1f));
            shadowMesh = Gem.Geo.Gen.FacetCopy(shadowMesh);
            Gem.Geo.Gen.CalculateTangentsAndBiNormals(shadowMesh);

            branch.Add(new Gem.Render.SceneGraph.MeshNode(shadowMesh, DropShadow, ShadowOrientation));

            Renderable = branch;
        }

        public override void Update(World World)
        {
            var downRay = new Ray(Orientation.Position + new Vector3(0,0,1), new Vector3(0, 0, -1));
            var floorHit = World.NavMesh.RayIntersection(downRay);
            if (floorHit != null)
                ShadowOrientation.Position = downRay.Position + (downRay.Direction * floorHit.distance) - new Vector3(0, 0, 0.01f);

        }
    }
}
