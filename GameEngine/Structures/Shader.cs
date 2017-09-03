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

namespace GameEngine.Structures
{
    internal class Shader
    {
        public VertexShader VertexShader { get; private set; }
        public PixelShader PixelShader { get; private set; }
        public InputLayout InputLayout { get; private set; }

        public Shader(VertexShader vertexShader, PixelShader pixelShader, InputLayout inputLayout)
        {
            this.VertexShader = vertexShader;
            this.PixelShader = pixelShader;
            this.InputLayout = inputLayout;
        }

        public void Dispose()
        {
            VertexShader.Dispose();
            PixelShader.Dispose();
            InputLayout.Dispose();
        }

    }
}
