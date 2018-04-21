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

namespace GameEngine
{
    internal static class ShaderManager
    {
        public static Shader BasicShader { get; private set; }
        public static Shader BasicInstanceShader { get; private set; }
        public static Shader BasicLightShader { get; private set; }
        public static Shader UIShader { get; private set; }

        internal static void Init()
        {
            //Basic shader
            {
                //Trim the first 3 bytes because Visual Studio adds 3 bad characters at the start of the file
                byte[] trimmedBytes = Properties.Resources.BasicShader.SubArray(3);

                InputElement[] elems = new InputElement[]
                    {
                        new InputElement("POSITION",0,Format.R32G32B32A32_Float,-1,0),
                    };

                Shader basicShader = Shader.Create(trimmedBytes, elems);
                ShaderManager.BasicShader = basicShader;
            }

            //Basic Instance shader
            {
                byte[] trimmedBytes = Properties.Resources.BasicInstanceShader.SubArray(3);
                InputElement[] elems = new InputElement[]
                {
                    new InputElement("POSITION",0,Format.R32G32B32A32_Float,-1,0,InputClassification.PerVertexData,0),

                    new InputElement("INSTANCEID",0,Format.R32_UInt,-1,1,InputClassification.PerInstanceData,1)
                };

                Shader basicInstanceShader = Shader.Create(trimmedBytes, elems);
                ShaderManager.BasicInstanceShader = basicInstanceShader;
            }

            {
                byte[] trimmedBytes = Properties.Resources.BasicLightShader.SubArray(3);
                InputElement[] elems = new InputElement[]
                {
                    new InputElement("POSITION", 0,Format.R32G32B32A32_Float,-1,0),
                    new InputElement("COLOR",0,Format.R32G32B32A32_Float,-1,0),
                    new InputElement("NORMAL",0,Format.R32G32B32_Float,-1,0)
                };

                Shader basicLightShader = Shader.Create(trimmedBytes, elems);
                ShaderManager.BasicLightShader = basicLightShader;
            }
        }

        internal static void Dispose()
        {
            Shader.DisposeAll();
        }

    }
}
