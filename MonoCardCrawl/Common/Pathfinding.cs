using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gem
{
    /// <summary>
    /// A generic implementation of A*
    /// </summary>
    /// <typeparam name="NODE"></typeparam>
    public class Pathfinding<NODE> where NODE: class
    {
        private Func<NODE, List<NODE>> EnumerateNeighbors;
        private Func<NODE, NODE, float> Heuristic;

        public Pathfinding(Func<NODE, List<NODE>> EnumerateNeighbors, Func<NODE, NODE, float> Heuristic)
        {
            this.EnumerateNeighbors = EnumerateNeighbors;
            this.Heuristic = Heuristic;
        }

        public class PathfindingResult
        {
            public List<NODE> Path;
            public bool FoundPath = false;
        }

        internal enum NodeState
        {
            Open,
            Closed
        }

        internal class PathNode
        {
            internal PathNode Parent;
            internal float PathCost;
            internal NODE RealNode;
            internal NodeState state = NodeState.Open;
        }

        public PathfindingResult FindPath(NODE from, Func<NODE, bool> goal)
        {
            var nodes = new Dictionary<NODE, PathNode>();
            var head = new PathNode { Parent = null, RealNode = from, state = NodeState.Closed };
            nodes.Add(from, head);
            var openNodes = new List<PathNode>();

            var result = new PathfindingResult();
            result.FoundPath = false;

            while (head != null)
            {
                if (goal(head.RealNode))
                {
                    var pathEnd = head;
                    result.Path = new List<NODE>();
                    while (pathEnd != null)
                    {
                        result.Path.Add(pathEnd.RealNode);
                        pathEnd = pathEnd.Parent;
                    }
                    result.Path.Reverse();
                    result.FoundPath = true;
                    return result;
                }

                foreach (var newOpenNode in EnumerateNeighbors(head.RealNode))
                {
                    if (nodes.ContainsKey(newOpenNode)) continue;
                    var newNode = new PathNode { Parent = head, RealNode = newOpenNode, state = NodeState.Open };
                    nodes.Add(newOpenNode, newNode);
                    openNodes.Add(newNode); //TODO: Sort addition based on heuristic
                }

                if (openNodes.Count == 0) head = null;
                else
                {
                    head = openNodes[0];
                    head.state = NodeState.Closed;
                    openNodes.RemoveAt(0);
                }
            }

            result.FoundPath = false;
            return result;
        }
    }
}
