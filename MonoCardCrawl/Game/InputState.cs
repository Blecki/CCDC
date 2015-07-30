﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gem;
using Gem.Render;

namespace Game
{
    public class InputState
    {
        public virtual void EnterState(WorldScreen Game, World World) { }
        public virtual void HandleClick(WorldScreen Game, World World, CombatCell Cell) { }
        public virtual void Update(WorldScreen Game, World World) { }

    }
}