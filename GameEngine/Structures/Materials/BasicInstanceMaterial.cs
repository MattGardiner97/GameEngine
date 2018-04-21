using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using Color = SharpDX.Color;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace GameEngine
{
    public class BasicInstanceMaterial : Material
    {
        protected class BufferStatus
        {
            public Buffer VertexBuffer;
            public Buffer IndexBuffer;
            public Buffer Instancebuffer;
            public VertexBufferBinding[] BufferBinding;
            public bool RebuildBuffer = false;
        }

        private const int CONSTANT_BUFFER_SIZE = 1024;

        private Dictionary<Mesh, List<MeshRenderer>> _renderList = new Dictionary<Mesh, List<MeshRenderer>>();
        private Dictionary<Mesh, BufferStatus> _bufferStatusDict = new Dictionary<Mesh, BufferStatus>();
        private Buffer _colorBuffer;
        private Buffer _matrixBuffer;

        public Color _mainColor= Color.White;
        public Color MainColor
        {
            get { return _mainColor; }
            set { _mainColor = value; _color = value.ToVector4(); }
        }
        private Vector4 _color;

        public BasicInstanceMaterial() : base(ShaderManager.BasicInstanceShader)
        {
            _colorBuffer = Graphics.Current.CreateConstantBuffer(SharpDX.Utilities.SizeOf<Vector4>());
            _matrixBuffer = Graphics.Current.CreateConstantBuffer(SharpDX.Utilities.SizeOf<Matrix>() * CONSTANT_BUFFER_SIZE);
            Debug.WriteLine("Implement frustum culling");
        }

        Matrix[] _mat = new Matrix[CONSTANT_BUFFER_SIZE];
        public override void DrawAll()
        {

            //Vector4 _color = this.MainColor.ToVector4();
            Graphics.Current.UpdateConstantBuffer(ref _color, _colorBuffer);

            Graphics.Current.SetShader(this.Shader);
            Graphics.Current.SetVertexShaderConstantBuffer(0, _colorBuffer);
            Graphics.Current.SetVertexShaderConstantBuffer(1, _matrixBuffer);



            foreach (Mesh m in _renderList.Keys)
            {
                if (_bufferStatusDict[m].RebuildBuffer == true)
                {
                    Buffer vertBuffer = Graphics.Current.CreateVertexBuffer(m.Vertices);
                    Buffer indexBuffer = Graphics.Current.CreateIndexBuffer(m.Triangles);
                    Buffer instanceBuffer = Graphics.Current.CreateVertexBuffer(Enumerable.Range(0, _renderList[m].Count).ToArray());

                    VertexBufferBinding[] buffArray = new VertexBufferBinding[]
                        {
                            new VertexBufferBinding(vertBuffer,16,0),
                            new VertexBufferBinding(instanceBuffer,4,0)
                        };

                    BufferStatus s = _bufferStatusDict[m];

                    s.VertexBuffer = vertBuffer;
                    s.IndexBuffer = indexBuffer;
                    s.Instancebuffer = instanceBuffer;
                    s.BufferBinding = buffArray;
                    s.RebuildBuffer = false;
                }


                BufferStatus current = _bufferStatusDict[m];


                Graphics.Current.SetIndexBuffer(current.IndexBuffer);
                Graphics.Current.SetVertexBuffers(current.BufferBinding);

                int groupCount = _renderList[m].Count / CONSTANT_BUFFER_SIZE + 1;
                for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
                {
                    
                    for (int i = 0; i < Math.Min(_renderList[m].Count - (groupIndex * CONSTANT_BUFFER_SIZE), CONSTANT_BUFFER_SIZE); i++)
                    {
                        _mat[i] = _renderList[m][groupIndex * CONSTANT_BUFFER_SIZE + i].Transform.WorldMatrix * Graphics.Current.WorldViewProj;
                        _mat[i].Transpose();
                    }

                    Graphics.Current.UpdateConstantBuffer(_mat, _matrixBuffer);


                    Graphics.Current.DrawIndexedInstanced(m.Triangles.Length, CONSTANT_BUFFER_SIZE, 0, 0, groupIndex * CONSTANT_BUFFER_SIZE);
                }



            }


        }

        public override void AddMeshRenderer(MeshRenderer Renderer)
        {
            if (_renderList.ContainsKey(Renderer.Mesh) == false)
                _renderList.Add(Renderer.Mesh, new List<MeshRenderer>());
            else
                _renderList[Renderer.Mesh].Remove(Renderer);

            if (_bufferStatusDict.ContainsKey(Renderer.Mesh) == false)
                _bufferStatusDict.Add(Renderer.Mesh, new BufferStatus());

            _bufferStatusDict[Renderer.Mesh].RebuildBuffer = true;

            _renderList[Renderer.Mesh].Add(Renderer);
        }

        public override void RemoveMeshRenderer(MeshRenderer Renderer)
        {
            _renderList[Renderer.Mesh].Remove(Renderer);
            _bufferStatusDict[Renderer.Mesh].RebuildBuffer = true;
        }
    }
}
