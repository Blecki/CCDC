using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public interface IInteractive
    {
        PlayerAction GetClickAction();
    }

    public abstract class PlayerAction
    {
        public virtual int Cost { get { return 0; } }
        public virtual void CarryOut(Gem.PropertyBag Properties) { }
    }

    public class LambdaPlayerAction : PlayerAction
    {
        private int _cost;

        public override int Cost
        {
            get { return _cost; }
        }

        public Action<Gem.PropertyBag> Implementation;

        public LambdaPlayerAction(int Cost, Action<Gem.PropertyBag> Implementation)
        {
            this._cost = Cost;
            this.Implementation = Implementation;
        }

        public override void CarryOut(Gem.PropertyBag Properties)
        {
            Implementation(Properties);
        }
    }

    public class PlayerCommandActionProxy : PlayerAction
    {
        private Input.PlayerCommand Command;
        
        public override int Cost { get { return Command.EnergyCost; } }
        public override void CarryOut(Gem.PropertyBag Properties)
        {
            Command.ConsiderPerform(Properties);
        }

        public PlayerCommandActionProxy(Input.PlayerCommand Command)
        {
            this.Command = Command;
        }
    }
 
}
