using System;
using System.Collections.Generic;

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

namespace GameEngine
{
    public abstract class Material
    {
        //Internal colletion of materials which each have DrawAll() called from Graphics.Draw()
        internal static List<Material> _materialList = new List<Material>();
        internal Shader Shader { get; private set; }

        public abstract void DrawAll();

        public abstract void AddMeshRenderer(MeshRenderer Renderer);
        public abstract void RemoveMeshRenderer(MeshRenderer Renderer);

        public Material(Shader shader)
        {
            this.Shader = shader;
            _materialList.Add(this);
        }
    }
}
