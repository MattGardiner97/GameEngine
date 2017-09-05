using SharpDX;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class MeshRenderer : Component
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
        public Material Material { get; set; } = new BasicMaterial();

        public Vector4[] InputElements { get { return this.Material.GetInputElements(this.Mesh); } }


    }
}
