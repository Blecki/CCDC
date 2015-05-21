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
		#region Navigation

		public Gem.Geo.EdgeMesh NavigationMesh;

        public Gem.Pathfinding<Gem.Geo.EMFace> pathfinder = new Pathfinding<Gem.Geo.EMFace>(
            (f) =>
            {
                var r = new List<Gem.Geo.EMFace>();
                foreach (var e in f.edges)
                    foreach (var n in e.Neighbors)
                        if (n != null && !Object.ReferenceEquals(n, f) && !r.Contains(n)) r.Add(n);
                return r;
            }, (a,b) => 1);

		public void BuildNavigationMesh()
		{
			NavigationMesh = Grid.GenerateNavigationMesh();
			//NavigationMesh.Simplify();
		}

		internal PathFindingResult FindPath(Vector3 from, Vector3 to)
		{
			var r = new PathFindingResult();
			r.PathFound = false;
			var actorFace = NavigationMesh.RayIntersection(new Ray(from + Vector3.UnitZ, -Vector3.UnitZ));
			if (actorFace == null) return r;
			var destinationFace = NavigationMesh.FaceAt(to);
			if (destinationFace == null) return r;
			var path = pathfinder.FindPath(actorFace.face,
				(f) => { return Object.ReferenceEquals(f, destinationFace); });
			if (!path.FoundPath) return r;
			r.PathPoints.Add(from);
			for (int i = 0; i < path.Path.Count - 1; ++i)
			{
				var a = path.Path[i];
				var b = path.Path[i + 1];
				var sharedEdge = Gem.Geo.EdgeMesh.FindSharedEdge(a, b);
				if (sharedEdge == null) throw new InvalidProgramException("Pathfinder found invalid path");
				r.PathPoints.Add(NavigationMesh.FindEdgeCenter(sharedEdge));
			}
			r.PathPoints.Add(to);
			r.PathFound = true;

			r.PathPoints = SmoothPath(r.PathPoints, (l) => { r.DebugTracePoints = l; });
			return r;
		}

		internal List<Vector3> SmoothPath(List<Vector3> input, Action<List<Vector3>> DebugTracePoints = null)
		{
			var output = new List<Vector3>();
			var debugPoints = new List<Vector3>();
			output.Add(input[0]);
			for (int i = 1; i < input.Count - 1; ++i)
			{
				if (!NavigationMesh.CanTrace(output[output.Count - 1], input[i + 1],
					(v) => { debugPoints.Add(v); }))
					output.Add(input[i]);
			}
			output.Add(input[input.Count - 1]);
			if (DebugTracePoints != null) DebugTracePoints(debugPoints);
			return output;
		}

		#endregion

		public CellGrid Grid;
		public Dictionary<String, Tile> TileSet = new Dictionary<String, Tile>();

        public World(EpisodeContentManager Content)
        {
            Grid = new CellGrid(1, 1, 1);
        }
        



    }
}
