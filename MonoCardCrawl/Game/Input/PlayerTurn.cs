using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game.Input
{
    public class PlayerTurn : InputState
    {
        private Actor BoundActor;
        private int MaxEnergy;
        private Gem.Gui.GuiSceneNode Gui;
        private Gem.Gui.UIItem[,] EnergyBar;
        private PlayerAction HoverAction;

        private bool TurnEnded = false;

        public PlayerTurn(Actor BoundActor)
        {
            this.BoundActor = BoundActor;
        }

        private Gem.Gui.Shape MakeTopChevron(int Left, int Right, int Top, int Bottom, int Delta, bool LeftFlat, bool RightFlat)
        {
            return new Gem.Gui.QuadShape(
                new Vector2(Left, Top),
                new Vector2(Right, Top),
                RightFlat ? new Vector2(Right, Bottom) : new Vector2(Right + Delta, Bottom),
                LeftFlat ? new Vector2(Left, Bottom) : new Vector2(Left + Delta, Bottom));
        }

        private Gem.Gui.Shape MakeBottomChevron(int Left, int Right, int Top, int Bottom, int Delta, bool LeftFlat, bool RightFlat)
        {
            return new Gem.Gui.QuadShape(
                LeftFlat ? new Vector2(Left, Top) : new Vector2(Left + Delta, Top),
                RightFlat ? new Vector2(Right, Top) : new Vector2(Right + Delta, Top),
                new Vector2(Right, Bottom),
                new Vector2(Left, Bottom));
        }

        public override void EnterState(WorldScreen Game, World World)
        {
            //TODO: Run some kind of turn-begins rule.
            if (!BoundActor.Properties.TryGetPropertyAs("max-energy", out MaxEnergy))
                MaxEnergy = 8;

            BoundActor.Properties.Upsert("turn-energy", MaxEnergy);

            //Prepare turn GUI
            
            var guiMesh = Gem.Geo.Gen.CreatePatch(
                new Vector3[] {
                    new Vector3(0,-2,4),     new Vector3(6,-2,2), new Vector3(10,-2,2),     new Vector3(16,-2,4),
                    new Vector3(0,-2,2.66f), new Vector3(6,-2,1.33f), new Vector3(10,-2,1.33f), new Vector3(16,-2,2.66f),
                    new Vector3(0,-2,1.33f), new Vector3(6,-2,0.66f), new Vector3(10,-2,0.66f), new Vector3(16,-2,1.33f),
                    new Vector3(0,-2,0),     new Vector3(6,-2,0), new Vector3(10,-2,0),     new Vector3(16,-2,0)
                }, 8);

            Gui = new Gem.Gui.GuiSceneNode(
                guiMesh,
                Game.Main.GraphicsDevice, 512, 256);

            //Gui.LayoutScaling = new Vector2(4, 4);
            
            Gui.Orientation.Position = new Vector3(0, 2, 0);

            // Make this GUI 'always on top'
            Gui.RenderOnTop = true;
            Gui.DistanceBias = float.NegativeInfinity;

            Game.SceneGraph.Add(Gui);

            Gui.uiRoot.AddPropertySet(null, new Gem.Gui.GuiProperties { BackgroundColor = new Vector3(0, 0, 1), Transparent = true });


            var guardButton = new Gem.Gui.UIItem(
                new Gem.Gui.QuadShape(512 - 48 - 64, 128 - 32, 64, 32),
                new Gem.Gui.GuiProperties
                {
                    BackgroundColor = new Vector3(0.7f, 0.7f, 0.7f),
                    ClickAction = new LambdaPlayerAction(1, a => Guard()),
                    Image = Game.Main.EpisodeContent.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Content/guard"),
                    ImageTransform = Matrix.CreateTranslation(-(512-48-64), -(128-32), 0) * Matrix.CreateScale(1.0f / 64, 1.0f / 32, 1.0f)
                });
            guardButton.AddPropertySet(item => item.Hover, new Gem.Gui.GuiProperties
                {
                    BackgroundColor = new Vector3(0.9f, 0.9f, 0.9f)
                });

            Gui.uiRoot.AddChild(guardButton);

            var chevronLeftEdge = 48;
            var chevronRightEdge = 512 - 48;
            var chevronRange = chevronRightEdge - chevronLeftEdge;
            var chevronSize = (int)Math.Round((float)chevronRange / (float)MaxEnergy);

            EnergyBar = new Gem.Gui.UIItem[2,MaxEnergy];

            for (var chevronIndex = 0; chevronIndex < MaxEnergy; ++chevronIndex)
            { 
                var topShape = MakeTopChevron(
                    chevronLeftEdge,
                    chevronIndex == MaxEnergy - 1 ? chevronRightEdge : chevronLeftEdge + chevronSize,
                    128,
                    128 + 64,
                    16,
                    chevronIndex == 0,
                    chevronIndex == MaxEnergy - 1);

                var bottomShape = MakeBottomChevron(
                    chevronLeftEdge,
                    chevronIndex == MaxEnergy - 1 ? chevronRightEdge : chevronLeftEdge + chevronSize,
                    128 + 64,
                    128 + 128,
                    16,
                    chevronIndex == 0,
                    chevronIndex == MaxEnergy - 1);
             
                chevronLeftEdge += chevronSize;

                EnergyBar[0, chevronIndex] = new Gem.Gui.UIItem(topShape, new Gem.Gui.GuiProperties
                {
                    BackgroundColor = new Vector3(0.8f, 0, 0)
                });

                EnergyBar[1, chevronIndex] = new Gem.Gui.UIItem(bottomShape, new Gem.Gui.GuiProperties
                    {
                        BackgroundColor = new Vector3(0,0,0.2f)
                    });

                Gui.uiRoot.AddChild(EnergyBar[0, chevronIndex]);
                Gui.uiRoot.AddChild(EnergyBar[1, chevronIndex]);
            }

            PrepareCombatGrid(Game, World);
        }

        private void Guard()
        {
            //EnergyLeft -= 1;
            //BoundActor.Properties.Upsert("bonus-energy", EnergyLeft);
            TurnEnded = true;
        }

        public override void Covered(WorldScreen Game, World World)
        {

        }

        public override void Update(WorldScreen Game, World World)
        {
            if (TurnEnded)
            {
                Game.PopInputState();
                return;
            }

            if (Game.HoverNode is IInteractive)
            {
                HoverAction = (Game.HoverNode as IInteractive).GetClickAction();
                //if (HoverAction != null && HoverAction.Cost > EnergyLeft) HoverAction = null;
                if (HoverAction != null && Game.Main.Input.Check("CLICK"))
                {
                    HoverAction.CarryOut(Gem.PropertyBag.Create(
                        "actor", BoundActor,
                        "game", Game,
                        "world", World));
                }

            }
         
            int turnEnergy;
            if (!BoundActor.Properties.TryGetPropertyAs("turn-energy", out turnEnergy))
                turnEnergy = MaxEnergy;

            var energySpent = MaxEnergy - turnEnergy;

            for (int i = 0; i < energySpent; ++i)
                EnergyBar[0, i].Properties[0].Values.Upsert("bg-color", new Vector3(0.2f, 0, 0));
            for (int i = energySpent; i < MaxEnergy; ++i)
                EnergyBar[0, i].Properties[0].Values.Upsert("bg-color", new Vector3(0.7f, 0, 0));

            if (HoverAction != null && energySpent + HoverAction.Cost <= MaxEnergy)
                for (int i = energySpent; i < energySpent + HoverAction.Cost; ++i)
                    EnergyBar[0, i].Properties[0].Values.Upsert("bg-color", new Vector3(1, 0, 0));


        }

        public override void Exposed(WorldScreen Game, World World)
        {
            if (BoundActor.Properties.GetPropertyAs<int>("turn-energy", () => 0) == 0)
                Game.PopInputState();
            else
                PrepareCombatGrid(Game, World);
        }


        public override void LeaveState(WorldScreen Game, World World)
        {
            Game.SceneGraph.Remove(Gui);
        }

        private void PrepareCombatGrid(WorldScreen Game, World World)
        {
            // Setup the combat grid

            World.PrepareCombatGridForPlayerInput();
            var pathfinder = new Gem.Pathfinding<CombatCell>(c => new List<CombatCell>(c.Links.Select(l => l.Neighbor).Where(n => World.GlobalRules.ConsiderCheckRule("can-traverse", BoundActor, n) == SharpRuleEngine.CheckResult.Allow)), a => 1.0f);

            var path = pathfinder.Flood(World.CombatGrid.Cells[(int)BoundActor.Orientation.Position.X, (int)BoundActor.Orientation.Position.Y, (int)BoundActor.Orientation.Position.Z], c => false, c => 1.0f);

            var walkCommand = BoundActor.Properties.GetPropertyAsOrDefault<PlayerCommand>("walk-command");
            if (walkCommand != null)
            {
                foreach (var node in path.VisitedNodes)
                {
                    var commandProperties = Gem.PropertyBag.Create(
                        "actor", BoundActor,
                        "game", Game,
                        "world", World,
                        "cell", node.Key,
                        "path", node.Value);
                    
                    var canWalk = walkCommand.ConsiderCheck(commandProperties);
                    if (canWalk == SharpRuleEngine.CheckResult.Allow)
                    {
                        node.Key.Visible = true;
                        node.Key.Texture = 1;
                        node.Key.PathNode = node.Value;
                        var localNode = node;
                        node.Key.ClickAction = new LambdaPlayerAction((int)node.Value.PathCost, a =>
                        {
                            walkCommand.ConsiderPerform(commandProperties);
                        });
                    }
                }
            }
        }
    }
}
