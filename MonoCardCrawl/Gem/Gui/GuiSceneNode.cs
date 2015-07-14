using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gem;
using Gem.Gui;

namespace Gem.Gui
{
    public class SceneNode : Render.SceneGraph.ISceneNode
    {
        private GuiDriver module = null;
        internal Render.Cameras.OrthographicCamera uiCamera = null;
        public UIItem uiRoot = null;
        internal RenderTarget2D renderTarget = null;
        private Geo.CompiledModel quadModel = null;
		public Matrix Offset = Matrix.Identity;

        internal bool MouseHover = false;
        internal int LocalMouseX = 0;
        internal int LocalMouseY = 0;

        public SceneNode(GraphicsDevice device, GuiDriver module, int width, int height, Euler Euler = null)
        {
            this.module = module;
            this.Orientation = Euler;
            if (this.Orientation == null) this.Orientation = new Euler();

            uiCamera = new Render.Cameras.OrthographicCamera(new Viewport(0, 0, width, height));
            uiRoot = new UIItem(new Rectangle(0, 0, width, height));

            uiCamera.focus = new Vector2(width / 2, height / 2);

            renderTarget = new RenderTarget2D(device, uiCamera.Viewport.Width, uiCamera.Viewport.Height);
            quadModel = Geo.CompiledModel.CompileModel(Geo.Gen.FacetCopy(Geo.Gen.CreateQuad()), device);
			uiRoot.Properties.Add(new UIItemProperties(null, module.defaultSettings));
        }

        public void ClearUI() { uiRoot.children.Clear(); }

        private static float ScalarProjection(Vector3 A, Vector3 B)
        {
            return Vector3.Dot(A, B) / B.Length();
        }

        public override void CalculateLocalMouse(Ray MouseRay)
        {
            MouseHover = false;

            var verts = new Vector3[3];
            verts[0] = new Vector3(-0.5f, -0.5f, 0);
            verts[1] = new Vector3(0.5f, -0.5f, 0);
            verts[2] = new Vector3(-0.5f, 0.5f, 0);

            for (int i = 0; i < 3; ++i)
                verts[i] = Vector3.Transform(verts[i], WorldTransform);

            var distance = MouseRay.Intersects(new Plane(verts[0], verts[1], verts[2]));
            if (distance == null || !distance.HasValue) return;
            if (distance.Value < 0) return; //GUI plane is behind camera
            var interesectionPoint = MouseRay.Position + (MouseRay.Direction * distance.Value);

            var x = ScalarProjection(interesectionPoint - verts[0], verts[1] - verts[0]) / (verts[1] - verts[0]).Length();
            var y = ScalarProjection(interesectionPoint - verts[0], verts[2] - verts[0]) / (verts[2] - verts[0]).Length();

            LocalMouseX = (int)(x * uiCamera.Viewport.Width);
            LocalMouseY = (int)(y * uiCamera.Viewport.Height);

            MouseHover = true;
        }

        public override void PreDraw(float ElapsedSeconds, Render.RenderContext Context)
        {
            module.DrawRoot(uiRoot, uiCamera, renderTarget);
        }

        public override void Draw(Render.RenderContext Context)
        {
            Context.Color = Vector3.One;
            Context.Texture = renderTarget;
            Context.NormalMap = Context.NeutralNormals;
            Context.World = WorldTransform;            
            Context.ApplyChanges();
            Context.Draw(quadModel);
        }

        public void DrawFlat(Render.RenderContext context, Gem.Render.Cameras.OrthographicCamera Camera)
        {
            var uiContext = module.GetRenderContext();
            uiContext.Camera = Camera;
            uiContext.BeginScene(null, false);
            uiRoot.Render(uiContext);
        }
    }
}
