using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gem.Geo
{
    public partial class Mesh
    {
        static private float AngleBetweenVectors(Vector3 A, Vector3 B)
        {
            A.Normalize();
            B.Normalize();
            float DotProduct = Vector3.Dot(A, B);
            DotProduct = MathHelper.Clamp(DotProduct, -1.0f, 1.0f);
            float Angle = (float)System.Math.Acos(DotProduct);
            return Angle;
        }

        public static bool IsPointOnFace(Vector3 p, Vector3[] Verticies)
        {
            var accumulatedAngle = 0.0f;

            for (int first = 0; first < 3; ++first)
            {
                var second = (first == 2 ? 0 : first + 1);
                var angle = AngleBetweenVectors(Verticies[first] - p, Verticies[second] - p);
                if (angle < 0) angle *= -1;
                accumulatedAngle += angle;
            }

            if (System.Math.Abs((System.Math.PI * 2) - accumulatedAngle) < 0.1f) 
                return true;
            return false;
        }

        public class RayIntersectionResult
        {
            public bool Intersects;
            public float Distance;
            public Object Tag;
        }

        public RayIntersectionResult RayIntersection(Ray ray)
        {
            var closestIntersection = new RayIntersectionResult { Distance = float.PositiveInfinity, Intersects = false };

            for (int vindex = 0; vindex < indicies.Length; vindex += 3)
            {
                var triangleVerts = indicies.Skip(vindex).Take(3).Select(i => verticies[i].Position).ToArray();
                var plane = new Plane(triangleVerts[0], triangleVerts[1], triangleVerts[2]);
                var intersectionDistance = ray.Intersects(plane);
                if (intersectionDistance.HasValue && intersectionDistance.Value < closestIntersection.Distance)
                {
                    var intersectionPoint = ray.Position + (ray.Direction * intersectionDistance.Value);
                    if (IsPointOnFace(intersectionPoint, triangleVerts))
                    {
                        closestIntersection.Distance = intersectionDistance.Value;
                        closestIntersection.Intersects = true;
                    }
                }
            }

            return closestIntersection;
        }
    }
}
