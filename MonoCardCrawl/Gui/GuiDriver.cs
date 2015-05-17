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
    public class GuiDriver
    {
        private GraphicsDevice device = null;
        private Render.ImmediateMode2d uiRenderer = null;
        public Dictionary<String, Object> defaultSettings = null;
        public Render.ImmediateMode2d GetRenderContext() { return uiRenderer; }

        public GuiDriver(GraphicsDevice device, EpisodeContentManager Content)
        {
            this.device = device;
            uiRenderer = new Render.ImmediateMode2d(device);

            defaultSettings = new Dictionary<String, Object>();
            defaultSettings.Add("BG-COLOR", new Vector3(0, 0, 0));
            defaultSettings.Add("TEXT-COLOR", new Vector3(1, 1, 1));
            defaultSettings.Add("FG-COLOR", new Vector3(1, 1, 1));
            defaultSettings.Add("HIDDEN-CONTAINER", null);
            defaultSettings.Add(    "FONT", new BitmapFont(Content.Load<Texture2D>("Content/small-font"), 6, 8, 6));
        }

        public void DrawRoot(UIItem root, Render.Cameras.OrthographicCamera camera, RenderTarget2D target)
        {
            uiRenderer.Camera = camera;
            uiRenderer.BeginScene(target);
            root.Render(uiRenderer);
        }

        public void DrawRenderable(Renderable renderable)
        {
            uiRenderer.Camera = renderable.uiCamera;
            uiRenderer.BeginScene(renderable.renderTarget);
            renderable.uiRoot.Render(uiRenderer);
        }

        public Renderable MakeGUI(UInt32 ID, int w, int h)
        {
            return new Renderable(device, this, w, h, null);
        }

        public List<List<Object>> Update(float elapsedSeconds, Input Input, Renderable guiNode)
        {
            var mousePressed = Input.Check("click");
            var events = new List<List<Object>>();
            guiNode.uiRoot.HandleMouseEx(guiNode.MouseHover, guiNode.LocalMouseX, guiNode.LocalMouseY, mousePressed,
               (olist) => { events.Add(olist); });
            guiNode.MouseHover = false;
            return events;
        }

        public List<List<Object>> FlatUpdate(float elapsedSeconds, Input Input, Renderable node)
        {
            var mouse = Input.QueryAxis("primary");
            node.MouseHover = true;
                node.LocalMouseX = (int)(mouse.X - node.uiRoot.rect.X);
                node.LocalMouseY = (int)(mouse.Y - node.uiRoot.rect.Y);
            return Update(elapsedSeconds, Input, node);
        }

    }
}
