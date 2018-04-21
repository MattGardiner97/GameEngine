using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

using SharpDX;
using Color = SharpDX.Color;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D11;

namespace GameEngine
{
    public class BasicMaterial : Material
    {
        private List<MeshRenderer> _renderList = new List<MeshRenderer>();

        private Buffer _matrixBuffer;
        private Buffer _colorBuffer;

        public Color MainColor { get; set; } = Color.White;

        public override void DrawAll()
        {
            foreach(MeshRenderer mr in _renderList)
            {
                Mesh m = mr.Mesh;

                Matrix _matrix = mr.Transform.WorldMatrix * Graphics.Current.WorldViewProj;
                _matrix.Transpose();
                Vector4 _color = MainColor.ToVector4();
                Graphics.Current.UpdateConstantBuffer(ref _matrix, _matrixBuffer);
                Graphics.Current.UpdateConstantBuffer(ref _color, _colorBuffer);

                Graphics.Current.SetShader(this.Shader);
                Graphics.Current.SetVertexShaderConstantBuffer(0, _matrixBuffer);
                Graphics.Current.SetVertexShaderConstantBuffer(1, _colorBuffer);

                Buffer vertexBuffer = Graphics.Current.CreateVertexBuffer(m.Vertices);
                Buffer indexBuffer = Graphics.Current.CreateIndexBuffer(m.Triangles);

                VertexBufferBinding[] buffArray = new VertexBufferBinding[] { new VertexBufferBinding(vertexBuffer, Shader.VertexShaderElementStride, 0) };

                Graphics.Current.SetIndexBuffer(indexBuffer);
                Graphics.Current.SetVertexBuffers(buffArray);

                Graphics.Current.DrawIndexed(m.Triangles.Length, 0, 0);

                vertexBuffer.Dispose();
                indexBuffer.Dispose();
            }
        }

        public override void AddMeshRenderer(MeshRenderer Renderer)
        {
            _renderList.Add(Renderer);
        }

        public override void RemoveMeshRenderer(MeshRenderer Renderer)
        {
            throw new NotImplementedException();
        }

        public BasicMaterial() : base(ShaderManager.BasicShader)
        {
            _matrixBuffer = Graphics.Current.CreateConstantBuffer(SharpDX.Utilities.SizeOf<Matrix>());
            _colorBuffer = Graphics.Current.CreateConstantBuffer(SharpDX.Utilities.SizeOf<Vector4>());
        }
    }
}