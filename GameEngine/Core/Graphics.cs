using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
//using System.Drawing;
using System.Windows.Forms;

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


namespace GameEngine
{
    public static class Graphics
    {
        private static RenderForm _form;
        private static SwapChainDescription _swapChainDescription;
        private static Device _device;
        private static SwapChain _swapChain;
        private static DeviceContext _context;
        private static Factory _factory;

        private static RenderTargetView _renderTargetView;
        private static Texture2D _backBuffer;
        private static Texture2D _depthBuffer;
        private static DepthStencilView _depthView;

        private static InputLayout _layout;
        private static ShaderBytecode _vsByteCode;
        private static VertexShader _vertexShader;
        private static ShaderBytecode _psByteCode;
        private static PixelShader _pixelShader;
        private static ShaderSignature _signature;

        private static Buffer _constantBuffer;

        //CAMERA DATA
        public static Vector3 CameraPosition = new Vector3(0,0,-5);
        public static Vector3 CameraTarget = Vector3.Zero;
        internal static Vector3 CameraUnitUp = Vector3.UnitY;

        private static Matrix _worldViewProj;
        private static Matrix _cameraWorld;
        private static Matrix _cameraView;
        private static Matrix _cameraProj;

        public static RenderForm Form
        {
            get
            {
                return _form;
            }
        }
        public static Color BackgroundColor
        {
            get;
            set;
        } = Color.Gray;
        public static Device GraphicsDevice
        {
            get
            {
                return _device;
            }
        }

        internal static void Dispose()
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

            _layout.Dispose();
            _vsByteCode.Dispose();
            _vertexShader.Dispose();
            _psByteCode.Dispose();
            _pixelShader.Dispose();
            _signature.Dispose();

            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _constantBuffer.Dispose();
        }

        internal static void Init()
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

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, _swapChainDescription, out _device, out _swapChain);

            _context = _device.ImmediateContext;

            _factory = _swapChain.GetParent<Factory>();
            _factory.MakeWindowAssociation(_form.Handle, WindowAssociationFlags.IgnoreAll);

            _vsByteCode = ShaderBytecode.CompileFromFile("Shader.hlsl","VS","vs_4_0");
            _vertexShader = new VertexShader(_device, _vsByteCode);

            _psByteCode = ShaderBytecode.CompileFromFile("Shader.hlsl", "PS", "ps_4_0");
            _pixelShader = new PixelShader(_device, _psByteCode);

            _signature = ShaderSignature.GetInputSignature(_vsByteCode);

            _layout = new InputLayout(_device, _signature, new InputElement[]
            {
                new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
                new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
            });

            //Defines a constant buffer holding the WorldViewProj for use by the VertexShader
            _constantBuffer = new Buffer(_device, SharpDX.Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            _context.InputAssembler.InputLayout = _layout;
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.VertexShader.Set(_vertexShader);
            _context.VertexShader.SetConstantBuffer(0, _constantBuffer);
            _context.PixelShader.Set(_pixelShader);

            _backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);
            _renderTargetView = new RenderTargetView(_device, _backBuffer);

            _depthBuffer = new Texture2D(_device, new Texture2DDescription()
            {
                Width = _form.ClientSize.Width,
                Height = _form.ClientSize.Height,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = new SampleDescription(1,0),
                Format = Format.D32_Float_S8X24_UInt,
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            _depthView = new DepthStencilView(_device, _depthBuffer);

            _context.Rasterizer.SetViewport(new Viewport(0, 0, Form.ClientSize.Width, Form.ClientSize.Height, 0.0f, 1.0f));
            _context.OutputMerger.SetTargets(_depthView, _renderTargetView);

            //Called when the program is exiting
            _form.FormClosing += (sender, args) =>
            {
                Engine.Shutdown();
            };

            _form.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                {
                    Engine.Shutdown();
                }
            };
        }

        private static List<Vector3> _vertList = new List<Vector3>();
        private static List<Vector4> _colorList = new List<Vector4>();
        private static List<int> _triList = new List<int>();
        private static Buffer _vertexBuffer;
        private static Buffer _indexBuffer;
        
        public static void Batch(Mesh m)
        {
            for(int i = 0; i <m.Vertices.Length;i++)
            {
                _vertList.Add(m.Vertices[i] + m.Transform.Position);
                _colorList.Add(new Vector4(m.Colors[i].R/255,m.Colors[i].G/255,m.Colors[i].B/255,1f));
            }

            int highestValue = _triList.Count == 0 ? 0 : _triList.Max() + 1;

            for(int i = 0; i < m.Triangles.Length;i++)
            {
                _triList.Add(m.Triangles[i] + highestValue);
            }
        }

        internal static void Draw()
        {
            _cameraView = Matrix.LookAtLH(CameraPosition, CameraTarget, CameraUnitUp);
            _cameraProj = Matrix.PerspectiveFovLH((float)(Math.PI / 4.0f), (float)(_form.ClientSize.Width / _form.ClientSize.Height), 1f, 1000f);
            _cameraWorld = Matrix.Identity;
            _worldViewProj = _cameraWorld * _cameraView * _cameraProj;

            _worldViewProj.Transpose();
            _context.UpdateSubresource(ref _worldViewProj, _constantBuffer);

            if (_vertList.Count != 0)
            {
                Vector4[] elementArray = new Vector4[_vertList.Count * 2];
                List<Vector4> inputElementList = new List<Vector4>();

                for(int i = 0;i < _vertList.Count;i++)
                {
                    Vector4 vertexPosition = new Vector4(_vertList[i], 1f);

                    inputElementList.Add(vertexPosition);
                    inputElementList.Add(_colorList[i]);
                }

                elementArray = inputElementList.ToArray();
                int[] triangleArray = _triList.ToArray();

                _vertexBuffer = Buffer.Create(_device, BindFlags.VertexBuffer, elementArray);
                _indexBuffer = Buffer.Create(_device, BindFlags.IndexBuffer, triangleArray);

                _context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, 32, 0));
                _context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            }

            _context.ClearRenderTargetView(_renderTargetView, BackgroundColor);
            _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            _context.DrawIndexed(_triList.Count, 0, 0);
            _swapChain.Present(0, PresentFlags.None);

            _vertList.Clear();
            _colorList.Clear();
            _triList.Clear();
        }

        internal static void Start()
        {
            RenderLoop.Run(_form, Engine.EngineLoop);
        }



    }
}
