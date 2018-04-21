using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace GameEngine.Structures
{
    public class BasicLightMaterial : Material
    {
        List<MeshRenderer> _renderList = new List<MeshRenderer>();

        struct bufferPerObject
        {
            public Matrix MVP;
            public Matrix Model;
        }

        struct Light
        {
            public Vector3 Direction;
            public float pad;
            public Vector4 ambient;
            public Vector4 diffuse;
        }

        struct bufferPerFrame
        {
            public Light light;
        }

        struct Vertex
        {
            Vector4 pos;
            Vector4 color;
            Vector3 normal;

            public Vertex(float x, float y, float z,float r, float g, float b, float a, float nx, float ny, float nz)
            {
                pos = new Vector4(x, y, z,1);
                color = new Vector4(r, g, b, a);
                normal = new Vector3(nx, ny, nz);
            }

            public Vertex(float x, float y, float z)
            {
                pos = new Vector4(x, y, z,1);
                color = new Vector4(1, 0, 0, 1);
                normal = new Vector3(x,y,z);
            }
        }

        private Buffer _perObjectBuffer;
        private Buffer _perFrameBuffer;

        public BasicLightMaterial() : base(ShaderManager.BasicLightShader)
        {
            _perObjectBuffer = Graphics.Current.CreateConstantBuffer(SharpDX.Utilities.SizeOf<bufferPerObject>());
            _perFrameBuffer = Graphics.Current.CreateConstantBuffer(SharpDX.Utilities.SizeOf<bufferPerFrame>());


        }

        public override void AddMeshRenderer(MeshRenderer Renderer)
        {
            _renderList.Add(Renderer);
        }

        public override void DrawAll()
        {
            Graphics.Current.SetShader(this.Shader);

            Light light = new Light();
            light.Direction = new Vector3(0.25f, 0.5f, -1f);
            light.ambient = new Vector4(0.2f, 0.2f, 0.2f, 1f);
            light.diffuse = new Vector4(1f, 1f, 1f, 1f);

            
            Vertex[] v = new Vertex[]
                {
                    //Bottom face
                    new Vertex(-1,-1,-1),
                    new Vertex(1,-1,-1),
                    new Vertex(-1,-1,1),
                    new Vertex(1,-1,1),

                    //Top Face
                    new Vertex(-1,1,-1),
                    new Vertex(1,1,-1),
                    new Vertex(-1,1,1),
                    new Vertex(1,1,1),

                    //Near Face
                    new Vertex(-1,-1,-1),
                    new Vertex(-1,1,-1),
                    new Vertex(1,1,-1),
                    new Vertex(1,-1,-1),

                    //Far Face
                    new Vertex(-1,-1,1),
                    new Vertex(1,-1,1),
                    new Vertex(-1,1,1),
                    new Vertex(1,1,1),

                    //Left Face
                    new Vertex(-1,-1,-1),
                    new Vertex(-1,-1,1),
                    new Vertex(-1,1,-1),
                    new Vertex(-1,1,1),

                    //Right face
                    new Vertex(1,-1,-1),
                    new Vertex(1,-1,1),
                    new Vertex(1,1,-1),
                    new Vertex(1,1,1)
                };
            int[] indices = new int[]
                {
                    0,1,2,
                    2,3,1,

                    4,6,5,
                    6,7,5,

                    8,9,11,
                    9,10,11,

                    12,13,14,
                    15,14,13,

                    16,17,19,
                    19,18,16,

                    20,22,21,
                    22,23,21
                };

            Buffer indexBuffer = Graphics.Current.CreateIndexBuffer(indices);
            Buffer vertexBuffer = Graphics.Current.CreateVertexBuffer(v);

            bufferPerFrame frameBuffer = new bufferPerFrame()
            {
                light = light
            };

            Matrix model = Matrix.Identity;
            Matrix mvp = model * Graphics.Current.WorldViewProj;
            model.Transpose();
            mvp.Transpose();
            bufferPerObject objectBuffer = new bufferPerObject()
            {
                Model = model,
                MVP = mvp
            };

            VertexBufferBinding[] buffArray = new VertexBufferBinding[] { new VertexBufferBinding(vertexBuffer,Shader.VertexShaderElementStride,0)};

            Graphics.Current.SetVertexBuffers(buffArray);
            Graphics.Current.SetIndexBuffer(indexBuffer);

            Graphics.Current.UpdateConstantBuffer(ref frameBuffer, _perFrameBuffer);
            Graphics.Current.UpdateConstantBuffer(ref objectBuffer,_perObjectBuffer);

            Graphics.Current.SetPixelShaderConstantBuffer(0, _perFrameBuffer);
            Graphics.Current.SetVertexShaderConstantBuffer(0, _perObjectBuffer);

            Graphics.Current.DrawIndexed(indices.Length, 0, 0);

            vertexBuffer.Dispose();
            indexBuffer.Dispose();
        }

        public override void RemoveMeshRenderer(MeshRenderer Renderer)
        {
            throw new NotImplementedException();
        }
    }
}
