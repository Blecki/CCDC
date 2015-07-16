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
    public class World
	{
		public CellGrid Grid;
		public Dictionary<String, Tile> TileSet = new Dictionary<String, Tile>();
        public List<Actor> Actors = new List<Actor>();

        public World(EpisodeContentManager Content)
        {
            Grid = new CellGrid(1, 1, 1);
        }

        public void SpawnActor(Actor Actor, Vector3 Position)
        {
            Actor.Orientation.Position = Position;
            Actors.Add(Actor);
        }
    }
}
