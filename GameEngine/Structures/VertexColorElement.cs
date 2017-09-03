using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using SharpDX;

using Color = SharpDX.Color;

namespace GameEngine
{
    public struct VertexColorElement
    {
        public Vector3 Position
        {
            get;set;
        }
        public Color MainColor
        {
            get;set;
        }

        public VertexColorElement(Vector3 position)
        {
            Position = position;
            MainColor = Color.Black;
        }
    }
}
