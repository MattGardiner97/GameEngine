using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace GameEngine
{
    public class Transform : Component
    {
        public Vector3 Position
        {
            get;
            set;
        }
        public Vector3 Scale
        {
            get;
            set;
        }
        public Vector3 Rotation
        {
            get;
            set;
        }

        public void Translate(Vector3 value)
        {
            Position = Position + value;
        }

    }
}
