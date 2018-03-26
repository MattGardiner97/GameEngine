using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace GameEngine
{
    public static class MathHelper
    {
        public static float CalculateDistance(Vector3 PointA, Vector3 PointB)
        {
            float a = PointA.X - PointB.X;
            float b = PointA.Y - PointB.Y;
            float c = PointA.Z - PointB.Z;

            float aS = a * a;
            float bS = b * b;
            float cS = c * c;

            float distance = (float)Math.Sqrt(aS + bS + cS);
            return (float)Math.Abs(distance);
        }

    }
}
