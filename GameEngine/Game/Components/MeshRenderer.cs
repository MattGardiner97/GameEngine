using SharpDX;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class MeshRenderer : Component
    {
        private static List<MeshRenderer> _dynamicMeshRenderList = new List<MeshRenderer>();
        internal static MeshRenderer[] GetDynamicMeshRenderers()
        {
            return _dynamicMeshRenderList.ToArray();
        }

        internal static Dictionary<Mesh, List<MeshRenderer>> _instancedMeshSet = new Dictionary<Mesh, List<MeshRenderer>>();


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
        private bool _instanced = false;
        public bool IsInstanced
        {
            get
            {
                return _instanced;
            }
            set
            {
                _instanced = value;

                if (value == true)
                {
                    if (_instancedMeshSet.ContainsKey(_mesh) == false)
                    {
                        _instancedMeshSet.Add(_mesh, new List<MeshRenderer>());
                        _instancedMeshSet[_mesh].Add(this);
                    }
                    else
                    {
                        //Make sure the value is not duplicated
                        _instancedMeshSet[_mesh].Remove(this);
                        _instancedMeshSet[_mesh].Add(this);
                    }
                }
                else
                {
                    if(_instancedMeshSet.ContainsKey(_mesh) == true)
                    {
                        _instancedMeshSet[_mesh].Remove(this);
                    }
                }

            }
        }

        public MeshRenderer()
        {
            _dynamicMeshRenderList.Add(this);
        }

        public Vector4[] InputElements { get { return this.Material.GetInputElements(this); } }

        internal override void Dispose()
        {
            Material.Dispose();
        }
    }
}
