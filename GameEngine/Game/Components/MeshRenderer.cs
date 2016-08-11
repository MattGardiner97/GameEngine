using System;
using System.Collections.Generic;
using System.Linq;
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

        public override void Draw()
        {
            Graphics.DrawMesh(_mesh);
        }

    }
}
