using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MonoCardCrawl
{
    public class CellGrid : Gem.Common.Grid3D<Cell>
    {
        private Cell CellProxy = new Cell();

        public CellGrid(int width, int height, int depth) : base(width, height, depth)
        {
        }

        public Cell CellAt(int X, int Y, int Z)
        {
            if (check(X, Y, Z)) return this[X, Y, Z];
            else return CellProxy;
        }

        public Gem.Geo.EdgeMesh GenerateNavigationMesh()
        {
            var meshes = new List<Gem.Geo.Mesh>();
            this.forRect(0, 0, 0, width, height, depth, (c, x, y, z) =>
                {
                    if (c.Solid && c.Tile != null && !CellAt(x, y, z + 1).Solid)
                    {
                        var mesh = c.Tile.NavigationMesh;
                        if (mesh != null)
                        {
                            mesh = Gem.Geo.Gen.Copy(mesh);
                            Gem.Geo.Gen.Transform(mesh, Matrix.CreateTranslation(x + 0.5f, y + 0.5f, z));
                            meshes.Add(mesh);
                        }
                    }
                });

            var rawNavMesh = Gem.Geo.Gen.Merge(meshes.ToArray());
            rawNavMesh = Gem.Geo.Gen.WeldCopy(rawNavMesh);

            var navMesh = new Gem.Geo.EdgeMesh(rawNavMesh);

            return navMesh;
        }
    }
}