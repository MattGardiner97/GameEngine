using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

using SharpDX;
using Color = SharpDX.Color;

namespace GameEngine
{
    public class BasicMaterial : Material
    {
        public Color MainColor { get; set; } = Color.White;

        internal override Vector4[] GetInputElements(Mesh mesh)
        {
            Vector4[] result = new Vector4[mesh.Vertices.Length * 2];
            Vector4 color = new Vector4(MainColor.R, MainColor.G, MainColor.B, MainColor.A);
            for (int i = 0; i < mesh.Vertices.Length; i++)
            {
                result[i * 2] = mesh.Vertices[i];
                result[i * 2 + 1] = color;
            }

            return result;
        }

        public BasicMaterial() : base(ShaderManager.BasicShader)
        {

        }
    }
}