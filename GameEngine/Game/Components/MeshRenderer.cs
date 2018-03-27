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
        private Material _material;
        public Material Material
        {
            get
            {
                return _material;
            }
            set
            {
                if (_material != null)
                    _material.RemoveMeshRenderer(this);

                value.AddMeshRenderer(this);
                _material = value;
            }
        }


        public MeshRenderer()
        {
            
        }
    }
}
