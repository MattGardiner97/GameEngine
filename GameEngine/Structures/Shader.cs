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
    public class Shader
    {
        private static List<Shader> _shaderPool = new List<Shader>();

        public VertexShader VertexShader { get; private set; }
        public PixelShader PixelShader { get; private set; }
        public InputLayout InputLayout { get; private set; }
        public int VertexShaderElementStride { get; private set; }

        private Shader(VertexShader VertexShader, PixelShader PixelShader, InputLayout InputLayout, int VertexShaderElementStride)
        {
            this.VertexShader = VertexShader;
            this.PixelShader = PixelShader;
            this.InputLayout = InputLayout;
            this.VertexShaderElementStride = VertexShaderElementStride;

            _shaderPool.Add(this);
        }

        internal static void DisposeAll()
        {
            foreach (Shader s in _shaderPool)
                s.Dispose();
        }

        internal void Dispose()
        {
            VertexShader.Dispose();
            PixelShader.Dispose();
            InputLayout.Dispose();
        }

        public static Shader Create(byte[] ShaderData, InputElement[] InputElements, string VertexShaderFunctionName = "VS", string PixelShaderFunctioName = "PS")
        {
            ShaderFlags flags = ShaderFlags.None;

#if DEBUG
            flags = ShaderFlags.Debug;
#endif

            var vsByteCode = ShaderBytecode.Compile(ShaderData, VertexShaderFunctionName, "vs_4_0",flags);
            VertexShader vs = new VertexShader(Graphics.Current.GraphicsDevice, vsByteCode);
            var psByteCode = ShaderBytecode.Compile(ShaderData, PixelShaderFunctioName, "ps_4_0",flags);
            PixelShader ps = new PixelShader(Graphics.Current.GraphicsDevice, psByteCode);

            InputLayout layout = new InputLayout(Graphics.Current.GraphicsDevice, ShaderSignature.GetInputSignature(vsByteCode), InputElements);

            int stride = InputElements.Sum(x =>
            {
                switch (x.Format)
                {
                    case Format.R32G32B32A32_Float:
                        return 16;
                    case Format.R32G32B32_Float:
                        return 12;
                    case Format.R32_UInt:
                        return 4;
                    default:
                        Debug.WriteLine($"Unable to compute stride of InputElement Format: {x.Format.ToString()}");
                        return 0;
                }
            });

            Shader s = new Shader(vs, ps, layout, stride);

            vsByteCode.Dispose();
            psByteCode.Dispose();

            return s;
        }

    }
}
