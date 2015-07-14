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
    public class WorldScreen : IScreen
    {
        public Input Input { get; set; }
        public Main Main { get; set; }

        EpisodeContentManager Content;
        float CameraDistance = -12;
        Vector3 CameraFocus = new Vector3(8.0f, 5.0f, 3.0f);
        public RenderContext RenderContext { get; private set; }
        public Gem.Render.Cameras.FreeCamera Camera { get; private set; }
        private World World;
        private Gem.Geo.EMFace HitFace;
        private Gem.Render.SceneGraph.BranchNode SceneGraph;
        private EditorGrid Grid;

        public WorldScreen()
        {
        }

        public void Begin()
        {
            Content = new EpisodeContentManager(Main.EpisodeContent.ServiceProvider, "Content");
            
            World = new World(Content);
            World.Grid.resize(16, 10, 16);

            RenderContext = new RenderContext(Content.Load<Effect>("draw"), Main.GraphicsDevice);
            Camera = new Gem.Render.Cameras.FreeCamera(new Vector3(0, 0, 0), Vector3.UnitY, Vector3.UnitZ, Main.GraphicsDevice.Viewport);
            RenderContext.Camera = Camera;

            var BlockTile = new Game.Tiles.BlockTile(RenderContext.White, Main.GraphicsDevice);
            var FloorTile = new Game.Tiles.FloorTile(Content.Load<Texture2D>("floor01"), Main.GraphicsDevice);
            
            World.Grid.ForEachTile((t, x, y, z) =>
                {
                    if (x == 0 || x == 15 || y == 9)
                        t.Tile = BlockTile;
                    else if (z == 0)
                        t.Tile = FloorTile;
                });

            World.Grid.CellAt(7, 3, 0).Tile = BlockTile;

            var shadow = Content.Load<Texture2D>("shadow");

            SceneGraph = new Gem.Render.SceneGraph.BranchNode();

            World.SpawnActor(new Game.Actors.QuadActor(Content.Load<Texture2D>("link01"), Content.Load<Texture2D>("normal-map"), shadow), new Vector3(4, 4, 4.25f));
            
            Camera.Position = CameraFocus + new Vector3(0, -4, 3);
            Camera.LookAt(CameraFocus);
            Camera.Position = CameraFocus + (Camera.GetEyeVector() * CameraDistance);

            Main.Input.AddBinding("RIGHT", new KeyboardBinding(Keys.Right, KeyBindingType.Held));
            Main.Input.AddBinding("LEFT", new KeyboardBinding(Keys.Left, KeyBindingType.Held));
            Main.Input.AddBinding("UP", new KeyboardBinding(Keys.Up, KeyBindingType.Held));
            Main.Input.AddBinding("DOWN", new KeyboardBinding(Keys.Down, KeyBindingType.Held));
            Main.Input.AddBinding("CLICK", new MouseButtonBinding("RightButton", KeyBindingType.Pressed));
            Main.Input.AddBinding("GRID-UP", new KeyboardBinding(Keys.Q, KeyBindingType.Pressed));
            Main.Input.AddBinding("GRID-DOWN", new KeyboardBinding(Keys.E, KeyBindingType.Pressed));

            Main.Input.AddBinding("CAMERA-DISTANCE-TOGGLE", new KeyboardBinding(Keys.R, KeyBindingType.Held));

            Main.ScriptBuilder.DeriveScriptsFrom("MonoCardCrawl.ScriptBase");

            World.NavMesh = WorldModel.GenerateNavigationMesh(World.Grid);

            var StaticWorld = WorldModel.CreateStaticGeometryBuffers(World, Main.GraphicsDevice);
            SceneGraph.Add(StaticWorld);

            Grid = new EditorGrid(Main.GraphicsDevice, 16, 10);
            Grid.Orientation.Scale = new Vector3(16, 10, 1.0f);
            SceneGraph.Add(Grid);
        }

        public void End()
        {
        }

        public void Update(float elapsedSeconds)
        {
            if (Main.Input.Check("GRID-UP")) Grid.Orientation.Position.Z += 1;
            if (Main.Input.Check("GRID-DOWN")) Grid.Orientation.Position.Z -= 1;

            if (Main.Input.Check("RIGHT")) Camera.Yaw(elapsedSeconds);
            if (Main.Input.Check("LEFT")) Camera.Yaw(-elapsedSeconds);
            if (Main.Input.Check("UP")) Camera.Pitch(elapsedSeconds);
            if (Main.Input.Check("DOWN")) Camera.Pitch(-elapsedSeconds);

            if (Main.Input.Check("CLICK"))
            {
                var pickVector = Camera.Unproject(new Vector3(Main.Input.QueryAxis("MAIN"), 0));
                var hitFace = World.NavMesh.RayIntersection(new Ray(Camera.Position, pickVector - Camera.Position));
                if (hitFace != null && hitFace.face != null)
                    this.HitFace = hitFace.face;

            }

            if (Main.Input.Check("CAMERA-DISTANCE-TOGGLE")) CameraDistance = -12.0f;
            else CameraDistance = -24.0f;

            Camera.Position = CameraFocus + (Camera.GetEyeVector() * CameraDistance);

            foreach (var actor in World.Actors)
                actor.Update(World);
        }

        public void Draw(float elapsedSeconds)
        {
            RenderContext.Color = Vector3.One;
            RenderContext.LightingEnabled = true;

            foreach (var actor in World.Actors.Where(a => a.Renderable != null))
            {
                actor.Renderable.UpdateWorldTransform(Matrix.Identity);
                actor.Renderable.PreDraw(elapsedSeconds, RenderContext);
            }

            var pickVector = Camera.Unproject(new Vector3(Main.Input.QueryAxis("MAIN"), 0));
            var pickRay = new Ray(Camera.Position, pickVector - Camera.Position);
            SceneGraph.CalculateLocalMouse(pickRay);

            SceneGraph.UpdateWorldTransform(Matrix.Identity);
            SceneGraph.PreDraw(elapsedSeconds, RenderContext);

            RenderContext.SetLight(0, new Vector3(2.5f, 2.5f, 2.5f), 10, new Vector3(1, 0, 0));
            RenderContext.SetLight(1, new Vector3(8.5f, 5.5f, 7.5f), 10, new Vector3(0, 1, 0));
            RenderContext.SetLight(2, new Vector3(13.5f, 3.5f, 3.5f), 10, new Vector3(0, 0, 1));
            RenderContext.ActiveLightCount = 3;

            RenderContext.NormalMap = RenderContext.NeutralNormals;
            Main.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0xFFFFFF, 0);

            SceneGraph.Draw(RenderContext);

            foreach (var actor in World.Actors.Where(a => a.Renderable != null))
                actor.Renderable.Draw(RenderContext);

            RenderContext.World = Matrix.Identity;
            RenderContext.Texture = RenderContext.White;
            World.NavMesh.DebugRender(RenderContext);
            if (HitFace != null) 
                World.NavMesh.DebugRenderFace(RenderContext, HitFace);
        }
    }
}
