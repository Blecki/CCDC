using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gem;
using Gem.Render;

namespace Game.Input
{
    class SelectTile : InputState
    {
        public Actor BoundActor;

        public SelectTile(Actor BoundActor)
        {
            this.BoundActor = BoundActor;
        }

        public override void EnterState(WorldScreen Game, World World)
        {
            World.PrepareCombatGridForPlayerInput();
            var pathfinder = new Pathfinding<CombatCell>(c => new List<CombatCell>(c.Links.Select(l => l.Neighbor).Where(n => World.GlobalRules.ConsiderCheckRule("can-traverse", BoundActor, n) == SharpRuleEngine.CheckResult.Allow)), a => 1.0f);

            var path = pathfinder.Flood(World.CombatGrid.Cells[(int)BoundActor.Orientation.Position.X, (int)BoundActor.Orientation.Position.Y, (int)BoundActor.Orientation.Position.Z], c => false, c => 1.0f);

            foreach (var node in path.VisitedNodes)
            {
                var canWalk = Game.World.GlobalRules.ConsiderCheckRule("can-walk", BoundActor, node.Key.ParentCell.Tile, node.Value.PathCost);
                if (canWalk == SharpRuleEngine.CheckResult.Allow)
                {
                    node.Key.Visible = true;
                    node.Key.Texture = 1;
                    node.Key.PathNode = node.Value;
                }
            }
        }

        public override void HandleClick(WorldScreen Game, World World, CombatCell Cell)
        {
            if (Cell.Texture == 1) //Move player to cell.
            {
                var path = Cell.PathNode.ExtractPath();
                BoundActor.NextAction = new Actors.Actions.WalkPath(path);
                Game.NextState = new WaitForIdle(BoundActor);
            }
        }
    }
}