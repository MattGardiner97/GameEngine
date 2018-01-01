using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using Color = SharpDX.Color;

namespace GameEngine
{
    public class BasicInstanceMaterial : Material
    {
        public Color MainColor { get; set; } = Color.White;

        public BasicInstanceMaterial() : base (ShaderManager.BasicInstanceShader)
        {
        }

        internal override Vector4[] GetInputElements(MeshRenderer mr)
        {
            Vector4 instancePosition = (Vector4)mr.Transform.WorldPosition;
            Vector4 instanceColor = new Vector4(MainColor.R, MainColor.G, MainColor.B, MainColor.A);
            return new Vector4[] { instancePosition, instanceColor };
        }
    }
}
