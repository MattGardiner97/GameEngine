using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Mathematics;
using SharpDX.Windows;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using GameEngine.Structures;

namespace GameEngine
{
    public abstract class Material
    {
        internal Shader Shader { get; private set; }

        internal abstract Vector4[] GetInputElements(Mesh mesh);

        internal Material(Shader shader)
        {
            this.Shader = shader;
        }
    }
}
