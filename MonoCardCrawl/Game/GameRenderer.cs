using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gem;

namespace MonoCardCrawl
{
	public class ActorRenderTag
	{
		public Gem.Render.IRenderable Renderable;
		public Matrix GuiOrientation = Matrix.Identity;
		public Gem.Gui.Renderable GuiRenderable;
	}

    public class GameRenderer
    {
        private Gem.Render.Renderer Renderer = null;
        private EpisodeContentManager Content;
		private Gem.Render.IRenderable WorldRenderable;
		private World World;
		private GraphicsDevice Device;
		private Gem.Render.DebuggingContext Debug;
		private Gem.Gui.GuiDriver GuiDriver;

		public Gem.Render.ICamera Camera { get { return Renderer.Camera; } }
		public Gem.Render.RenderContext Context { get { return Renderer.GetContext(); } }
		public Gem.Render.Renderer BasicRenderer { get { return Renderer; } }

		public bool DisplayNavigationMesh = false;

		public GameRenderer(
			World World,
			Gem.Gui.GuiDriver GuiDriver,
			EpisodeContentManager Content,
			GraphicsDevice Device,
			Gem.Render.DebuggingContext Debug)
		{
			this.World = World;
			this.Content = Content;
			this.Device = Device;
			this.Debug = Debug;
			this.GuiDriver = GuiDriver;

			Renderer = new Gem.Render.Renderer(Device, Content);
			Renderer.Camera = new Gem.Render.Cameras.FreeCamera(new Vector3(0, 0, 0), Vector3.UnitY, Vector3.UnitZ,
				Device.Viewport);

            //World.ActorCreated += (actor) =>
            //    {
            //        var actorNode = MakeActorVisuals(actor.ID, actor.Orientation, actor.Template);
            //        actor.tag = new ActorRenderTag
            //        {
            //            Renderable = actorNode
            //        };
            //    };

            CreateWorldRenderable();
		}

		public void Update(float elapsedSeconds, Input Input)
		{

        }

        private void CreateWorldRenderable()
        {
                WorldRenderable = new Gem.Render.SceneGraph.Root(new TileGridNode(World.Grid));
        }

		public void Draw(float elapsedSeconds, Vector2 PickAxis)
		{
			var everything = new List<Gem.Render.IRenderable>();

				Renderer.PreDraw(elapsedSeconds, WorldRenderable);
				everything.Add(WorldRenderable);

            //foreach (var Actor in World.Actors)
            //{
            //    var actorTag = Actor.Value.tag as ActorRenderTag;
            //    Renderer.PreDraw(elapsedSeconds, actorTag.Renderable);
            //    if (actorTag.GuiRenderable != null)
            //    {
            //        Renderer.PreDraw(elapsedSeconds, actorTag.GuiRenderable);
            //        actorTag.GuiRenderable.Offset = actorTag.GuiOrientation;
            //    }

            //    everything.Add(actorTag.Renderable);
            //    if (actorTag.GuiRenderable != null) everything.Add(actorTag.GuiRenderable);
            //}

            if (Debug.Visualization != null) everything.Add(Debug.Visualization);

            if (DisplayNavigationMesh && World.NavigationMesh != null)
                foreach (var edge in World.NavigationMesh.Edges)
                    Renderer.AddDebugLine(
                        new VertexPositionColor(World.NavigationMesh.Verticies[edge.Verticies[0]], Color.Red),
                        new VertexPositionColor(World.NavigationMesh.Verticies[edge.Verticies[1]], Color.Red));

            //if (HiliteObject != null) HiliteObject.SetHilite(true);
			Renderer.Draw(everything, Gem.Render.Renderer.DrawModeFlag.Normal);
            //if (HiliteObject != null) HiliteObject.SetHilite(false);
		}
    }
}
