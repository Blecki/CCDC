using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Gem.Gui
{
    public class GuiProperties : PropertyBag
    {
        public Vector3 BackgroundColor
        {
            get { return GetPropertyAs<Vector3>("bg-color"); }
            set { Upsert("bg-color", value); }
        }

        public Vector3 TextColor
        {
            get { return GetPropertyAs<Vector3>("text-color"); }
            set { Upsert("text-color", value); }
        }

        public BitmapFont Font
        {
            get { return GetPropertyAs<BitmapFont>("font"); }
            set { Upsert("font", value); }
        }

        public bool Transparent
        {
            set { Upsert("transparent", value); }
        }

        public String Label
        {
            set { Upsert("label", value); }
        }
    }
}
