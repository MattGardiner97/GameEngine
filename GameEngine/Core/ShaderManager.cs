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

using GameEngine.Structures;
using GameEngine.Utilities;

using Device = SharpDX.Direct3D11.Device;

namespace GameEngine
{
    internal static class ShaderManager
    {
        public static Shader BasicShader { get; private set; }
        public static Shader UIShader { get; private set; }

        internal static void Init(Device device)
        {
            //Basic shader
            {
                //Trim the first 3 bytes because Visual Studio adds 3 bad characters at the start of the file
                byte[] trimmedBytes = Properties.Resources.BasicShader.SubArray(3);
                var vsByteCode = ShaderBytecode.Compile(trimmedBytes, "VS", "vs_4_0");
                VertexShader vs = new VertexShader(device, vsByteCode);
                var psByteCode = ShaderBytecode.Compile(trimmedBytes, "PS", "ps_4_0");
                PixelShader ps = new PixelShader(device, psByteCode);

                InputLayout layout = new InputLayout(device, ShaderSignature.GetInputSignature(vsByteCode), new InputElement[]
                    {
                        new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                        new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
                    });

                Shader basicShader = new Shader(vs, ps, layout);
                ShaderManager.BasicShader = basicShader;

                vsByteCode.Dispose();
                psByteCode.Dispose();
            }

            //UI Shader
            {
                byte[] trimmedBytes = Properties.Resources.UIShader.SubArray(3);
                var vsByteCode = ShaderBytecode.Compile(trimmedBytes, "VS", "vs_4_0");
                VertexShader vs = new VertexShader(device, vsByteCode);
                var psByteCode = ShaderBytecode.Compile(trimmedBytes, "PS", "ps_4_0");
                PixelShader ps = new PixelShader(device, psByteCode);

                InputLayout layout = new InputLayout(device, ShaderSignature.GetInputSignature(vsByteCode), new InputElement[]
                    {
                        new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                        new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
                    });

                Shader uiShader = new Shader(vs, ps, layout);
                ShaderManager.UIShader = uiShader;

                vsByteCode.Dispose();
                psByteCode.Dispose();
            }
        }

        internal static void Dispose()
        {
            BasicShader.Dispose();
            UIShader.Dispose();
        }

    }
}
