using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gem;
using Gem.Render;

namespace MonoCardCrawl
{
    public class ModelScreen : IScreen
    {
        public Input Input { get; set; }
        public Main Main { get; set; }
        GameRenderer GameRenderer;

        EpisodeContentManager Content;

        public ModelScreen()
        {
        }

        public void Begin()
        {
            Content = new EpisodeContentManager(Main.EpisodeContent.ServiceProvider, "Content");
            var Texture = Content.Load<Texture2D>("stone_block");

            var Mesh = Gem.Geo.Gen.CreateTexturedFacetedCube();
            Mesh.PrepareLineIndicies();

            var world = new World(Content);
            world.Grid.resize(2, 2, 1);
            GameRenderer = new GameRenderer(world, new Gem.Gui.GuiDriver(Main.GraphicsDevice, Main.EpisodeContent), Content, Main.GraphicsDevice, new DebuggingContext());
            world.Grid.CellAt(0, 0, 0).Tile = new Tile { RenderMesh = Mesh, Texture = Texture };
            world.Grid.CellAt(1, 1, 0).Tile = new Tile { RenderMesh = Mesh, Texture = Texture };

            var fcam = GameRenderer.Camera as Gem.Render.Cameras.FreeCamera;
            fcam.Position = new Vector3(0.5f, -4, 3);
            fcam.LookAt(new Vector3(0.5f, 0, 0));
            fcam.Position = new Vector3(0.5f, 0, 0) + (fcam.GetEyeVector() * -5);

            Main.Input.AddBinding("RIGHT", new KeyboardBinding(Keys.Right, KeyBindingType.Held));
            Main.Input.AddBinding("LEFT", new KeyboardBinding(Keys.Left, KeyBindingType.Held));
            Main.Input.AddBinding("UP", new KeyboardBinding(Keys.Up, KeyBindingType.Held));
            Main.Input.AddBinding("DOWN", new KeyboardBinding(Keys.Down, KeyBindingType.Held));

            Main.ScriptBuilder.DeriveScriptsFrom("MonoCardCrawl.ScriptBase");
        }

        public void End()
        {
        }

        public void Update(float elapsedSeconds)
        {
            if (Main.Input.Check("RIGHT"))
            {
                var cam = GameRenderer.Camera as Gem.Render.Cameras.FreeCamera;
                cam.Yaw(elapsedSeconds);
                cam.Position = new Vector3(0.5f, 0, 0) + (cam.GetEyeVector() * -5);
            }

            if (Main.Input.Check("LEFT"))
            {
                var cam = GameRenderer.Camera as Gem.Render.Cameras.FreeCamera;
                cam.Yaw(-elapsedSeconds);
                cam.Position = new Vector3(0.5f, 0, 0) + (cam.GetEyeVector() * -5);
            }

            if (Main.Input.Check("UP"))
            {
                var cam = GameRenderer.Camera as Gem.Render.Cameras.FreeCamera;
                cam.Pitch(elapsedSeconds);
                cam.Position = new Vector3(0.5f, 0, 0) + (cam.GetEyeVector() * -5);
            }

            if (Main.Input.Check("DOWN"))
            {
                var cam = GameRenderer.Camera as Gem.Render.Cameras.FreeCamera;
                cam.Pitch(-elapsedSeconds);
                cam.Position = new Vector3(0.5f, 0, 0) + (cam.GetEyeVector() * -5);
            }

            GameRenderer.Update(elapsedSeconds, Main.Input);
        }

        public void Draw(float elapsedSeconds)
        {
            GameRenderer.Draw(elapsedSeconds, Vector2.Zero);
        }
    }
}
