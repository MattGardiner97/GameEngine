using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Mathematics;
using SharpDX.Windows;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace GameEngine
{
    public class Shape2D : Component
    {
        private Mesh _mesh;

        public Mesh Mesh
        {
            get
            {
                return _mesh;
            }
            set
            {
                _mesh = value;
                _mesh.Transform = GameObject.Transform;
            }
        }

        public override void Draw()
        {
            //Graphics.Batch(Mesh);
        }

    }

    
}
