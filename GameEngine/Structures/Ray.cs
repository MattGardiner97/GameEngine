using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Structures
{
    public struct Ray
    {
        public Vector3 Origin { get; private set; }
        public Vector3 Direction { get; private set; }
        public float Length { get; private set; }

        public Ray(Vector3 Origin, Vector3 Direction, float Length)
        {
            this.Origin = Origin;
            this.Direction = Direction;
            this.Length = Length;
        }
    }
}
