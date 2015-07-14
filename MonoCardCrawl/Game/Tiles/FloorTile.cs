﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoCardCrawl.Game.Tiles
{ 
    public class FloorTile : Tile
    {
        public FloorTile(Texture2D Texture, GraphicsDevice Device)
        {
            this.RenderMesh = Gem.Geo.Gen.CreateTexturedFacetedCube();
            Gem.Geo.Gen.Transform(RenderMesh, Matrix.CreateTranslation(0, 0, 0.5f));
            Gem.Geo.Gen.Transform(RenderMesh, Matrix.CreateScale(1, 1, 0.25f));
            
            this.Texture = Texture;

            this.NavigationMesh = Gem.Geo.Gen.CreateQuad();
            Gem.Geo.Gen.Transform(NavigationMesh, Matrix.CreateTranslation(0, 0, 0.25f));

            this.CompiledRenderMesh = Gem.Geo.CompiledModel.CompileModel(RenderMesh, Device);
        }

        public override bool Combinable(Tile Other)
        {
            return Object.ReferenceEquals(this.Texture, Other.Texture);
        }
    }
}