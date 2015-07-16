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
    public class CombatCell
    {

        public enum Direction
        {
            North,
            East,
            South,
            West
        }

        public struct Link
        {
            public Direction Direction;
            public CombatCell Neighbor;
        }

        public static Vector3 DirectionOffset(Direction Direction)
        {
            switch (Direction)
            {
                case Direction.North: return new Vector3(0, -1, 0);
                case Direction.East: return new Vector3(1, 0, 0);
                case Direction.South: return new Vector3(0, 1, 0);
                case Direction.West: return new Vector3(-1, 0, 0);
                default: throw new InvalidOperationException();
            }
        }

        public bool Visible = true;
        public Gem.Geo.Mesh Mesh;
        public Cell ParentCell;
        public Texture2D Texture;

        public List<Link> Links;
    }
}
