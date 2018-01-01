using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
//using System.Drawing;
using System.Windows.Forms;
using System.Linq;

using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Mathematics;
using SharpDX.Windows;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using Math = System.Math;

using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;

using GameEngine.Utilities;
using GameEngine.Structures;

namespace GameEngine
{
    public class Graphics
    {
        private Engine _parentEngine;
        private RenderForm _form;
        private SwapChainDescription _swapChainDescription;
        private Device _device;
        private SwapChain _swapChain;
        private DeviceContext _context;
        private Factory _factory;

        private RenderTargetView _renderTargetView;
        private Texture2D _backBuffer;
        private Texture2D _depthBuffer;
        private DepthStencilView _depthView;

        private Buffer _constantBuffer;
        private Buffer _instanceConstantBuffer;

        //private List<Mesh> _meshList = new List<Mesh>();

        //CAMERA DATA
        public Vector3 CameraPosition = new Vector3(0, 2, -3f);
        public Vector3 CameraTarget = Vector3.Zero;
        internal Vector3 CameraUnitUp = Vector3.UnitY;

        private Matrix _worldViewProj;
        //private Matrix _cameraWorld;
        private Matrix _cameraView;
        private Matrix _cameraProj;

        public RenderForm Form { get { return _form; } }
        public Color BackgroundColor { get; set; } = Color.Gray;
        public Device GraphicsDevice { get { return _device; } }

        //Statics
        public static Graphics Current { get; private set; }

        public Graphics(Engine ParentEngine)
        {
            _parentEngine = ParentEngine;
            Graphics.Current = this;
        }

        internal void Dispose()
        {
            _form.Dispose();
            _device.Dispose();
            _swapChain.Dispose();
            _context.Dispose();
            _factory.Dispose();

            _renderTargetView.Dispose();
            _backBuffer.Dispose();
            _depthBuffer.Dispose();
            _depthView.Dispose();

            _constantBuffer.Dispose();
            _instanceConstantBuffer.Dispose();
        }

        internal void Init()
        {
            _form = new RenderForm("Game Engine");

            _swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(_form.ClientSize.Width, _form.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = _form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

#if DEBUG
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, _swapChainDescription, out _device, out _swapChain);
#else
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, _swapChainDescription, out _device, out _swapChain);
            
#endif


            _context = _device.ImmediateContext;

            _factory = _swapChain.GetParent<Factory>();
            _factory.MakeWindowAssociation(_form.Handle, WindowAssociationFlags.IgnoreAll);

            //Defines a constant buffer holding the WorldViewProj for use by the VertexShader
            _constantBuffer = new Buffer(_device, SharpDX.Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.VertexShader.SetConstantBuffer(0, _constantBuffer);

            _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
            _renderTargetView = new RenderTargetView(_device, _backBuffer);

            _depthBuffer = new Texture2D(_device, new Texture2DDescription()
            {
                Width = _form.ClientSize.Width,
                Height = _form.ClientSize.Height,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1, 0),
                Format = Format.D32_Float_S8X24_UInt,
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            _depthView = new DepthStencilView(_device, _depthBuffer);

            _context.Rasterizer.SetViewport(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));
            _context.Rasterizer.State = new RasterizerState(_device, new RasterizerStateDescription()
            {
                CullMode = CullMode.Back,
                DepthBias = 0,
                DepthBiasClamp = 0,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0
            });

            _context.OutputMerger.SetTargets(_depthView, _renderTargetView);

            _instanceConstantBuffer = new Buffer(_device, SharpDX.Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            //Called when the program is exiting
            _form.FormClosing += (sender, args) =>
            {
                _parentEngine.Shutdown();
            };

            _form.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                {
                    _parentEngine.Shutdown();
                }
            };
        }

        internal void Draw()
        {
            CameraPosition = Camera.MainCamera.Transform.WorldPosition;
            CameraTarget = Camera.MainCamera.Transform.Forward;

            _cameraView = Matrix.LookAtLH(CameraPosition, CameraTarget, CameraUnitUp);
            _cameraProj = Matrix.PerspectiveFovLH((float)(Math.PI / 4.0f), (float)(_form.ClientSize.Width / _form.ClientSize.Height), 1f, 100f);

            Matrix viewProj = _cameraView * _cameraProj;

            _context.ClearRenderTargetView(_renderTargetView, BackgroundColor);
            _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);


            #region Test
            //DrawDynamic(viewProj);
            DrawInstanced(viewProj);
            #endregion

            _swapChain.Present(0, PresentFlags.None);
        }

        public class FastList<T>
        {
            private T[] _arr;
            private int _index = 0;

            public int Count { get { return _index; } }

            public FastList() : this(1)
            {

            }

            public FastList(int Capacity)
            {
                _arr = new T[Capacity];
            }

            public void Clear()
            {
                _index = 0;
            }

            public void Add(T Value)
            {
                if (_index >= _arr.Length)
                {
                    T[] old = _arr;
                    _arr = new T[old.Length * 2];
                    Array.Copy(old, _arr, old.Length);
                }
                _arr[_index++] = Value;
            }

            public void AddRange(T[] Values)
            {
                if (_index + Values.Length >= _arr.Length)
                {
                    int newSize = (_arr.Length + Values.Length) * 2;
                    T[] old = _arr;
                    _arr = new T[newSize];
                    Array.Copy(old, _arr, old.Length);
                }
                Array.Copy(Values, 0, _arr, _index, Values.Length);
                _index += Values.Length;
            }
            public void AddRange(IEnumerable<T> Values)
            {
                foreach (T v in Values)
                    Add(v);
            }

            public T[] ToArray()
            {
                return _arr;
            }
        }

        FastList<Vector4> vertList = new FastList<Vector4>();
        FastList<int> triList = new FastList<int>();
        int triMax = 0;
        private void DrawDynamic(Matrix viewProj)
        {
            vertList.Clear();
            triList.Clear();
            triMax = 0;

            Structures.Shader s = ShaderManager.BasicShader;
            _context.VertexShader.Set(s.VertexShader);
            _context.PixelShader.Set(s.PixelShader);
            _context.InputAssembler.InputLayout = s.InputLayout;

            MeshRenderer[] _meshArray = MeshRenderer.GetDynamicMeshRenderers();

            BoundingFrustum bf = new BoundingFrustum(viewProj);



            foreach (MeshRenderer mr in _meshArray)
            {
                if (MathHelper.CalculateDistance(mr.Transform.Position, CameraPosition) > 100)
                    continue;

                if (mr.Material == null)
                    continue;

                Vector4[] oldVerts = mr.Mesh.Vertices;
                Vector3 pos = mr.Transform.Position;
                bool shouldDraw = false;
                for (int i = 0; i < oldVerts.Length; i++)
                {
                    if (bf.Contains(pos + (Vector3)oldVerts[i]) != ContainmentType.Disjoint)
                    {
                        shouldDraw = true;
                        break;
                    }
                }

                if (shouldDraw == false)
                    continue;

                #region Transformation
                _worldViewProj = mr.Transform.WorldMatrix * _cameraView * _cameraProj;
                //_worldViewProj.Transpose();

                Vector4[] elems = mr.InputElements;

                for (int i = 0; i < elems.Length; i += 2)
                {
                    Vector4 elem = elems[i];

                    Matrix m = new Matrix();
                    m.Row1 = elem;

                    Vector4 transformed = (m * _worldViewProj).Row1;
                    vertList.Add(transformed);
                    vertList.Add(elems[i + 1]);
                }

                int[] triangles = mr.Mesh.Triangles;
                triangles = triangles.Select(x => x + triMax).ToArray();
                triMax += mr.Mesh.Triangles.Max() + 1;
                triList.AddRange(triangles);
                //foreach (int i in triangles)
                //    triList.Add(i);
                //foreach (int i in triangles)
                //    Add(ref triList, i, triListIndex++);



                //_context.UpdateSubresource(ref _worldViewProj, _constantBuffer);
                #endregion                

                //Buffer _vertexBuffer = Buffer.Create(_device, BindFlags.VertexBuffer, elems);
                //Buffer _indexBuffer = Buffer.Create(_device, BindFlags.IndexBuffer, mr.Mesh.Triangles);

                //_context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, 32, 0));
                //_context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);

                //_context.DrawIndexed(mr.Mesh.Triangles.Length, 0, 0);

                //_vertexBuffer.Dispose();
                //_indexBuffer.Dispose();
            }

            if (vertList.Count != 0)
            {
                Buffer _vertexBuffer = Buffer.Create(_device, BindFlags.VertexBuffer, vertList.ToArray());
                Buffer _indexBuffer = Buffer.Create(_device, BindFlags.IndexBuffer, triList.ToArray());

                _context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, 32, 0));
                _context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);

                _context.DrawIndexed(triList.Count, 0, 0);

                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();
            }
        }

        private void DrawInstanced(Matrix viewProj)
        {
            throw new Exception("Implement frustum culling");

            foreach (Mesh curMesh in MeshRenderer._instancedMeshSet.Keys)
            {
                Buffer vertBuffer = Buffer.Create(_device, BindFlags.VertexBuffer, curMesh.Vertices);
                Buffer indexBuffer = Buffer.Create(_device, BindFlags.IndexBuffer, curMesh.Triangles);

                var shaderGrouped = MeshRenderer._instancedMeshSet[curMesh].GroupBy(x => x.Material.Shader).ToDictionary(x => x.Key);

                foreach (Shader s in shaderGrouped.Keys)
                {
                    _context.VertexShader.Set(s.VertexShader);
                    _context.PixelShader.Set(s.PixelShader);
                    _context.InputAssembler.InputLayout = s.InputLayout;
                    _context.VertexShader.SetConstantBuffer(0, _instanceConstantBuffer);

                    Matrix __world = Matrix.Identity;
                    Matrix MVP = __world * _cameraView * _cameraProj;
                    MVP.Transpose();
                    _context.UpdateSubresource(ref MVP, _instanceConstantBuffer);

                    MeshRenderer[] mrList = shaderGrouped[s].ToArray();

                    List<Vector4> instanceList = new List<Vector4>();

                    foreach (MeshRenderer mr in mrList)
                    {
                        instanceList.AddRange(mr.Material.GetInputElements(mr));
                    }

                    Buffer instanceBuffer = Buffer.Create(_device, BindFlags.VertexBuffer, instanceList.ToArray());

                    VertexBufferBinding[] buffArray = new VertexBufferBinding[]
                        {
                            new VertexBufferBinding(vertBuffer,16,0),
                            new VertexBufferBinding(instanceBuffer,32,0)
                        };

                    _context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
                    _context.InputAssembler.SetVertexBuffers(0, buffArray);
                    _context.VertexShader.SetConstantBuffer(0, _instanceConstantBuffer);

                    _context.DrawIndexedInstanced(curMesh.Triangles.Length, mrList.Length, 0, 0, 0);
                }

                vertBuffer.Dispose();
                indexBuffer.Dispose();
            }
        }
    }
}
