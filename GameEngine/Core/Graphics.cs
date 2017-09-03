using System;
using System.Collections.Generic;

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

        private List<Mesh> _meshList = new List<Mesh>();

        //CAMERA DATA
        public Vector3 CameraPosition = new Vector3(3, 3, -3);
        public Vector3 CameraTarget = Vector3.Zero;
        internal Vector3 CameraUnitUp = Vector3.UnitY;

        private Matrix _worldViewProj;
        private Matrix _cameraWorld;
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

            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, _swapChainDescription, out _device, out _swapChain);

            _context = _device.ImmediateContext;

            _factory = _swapChain.GetParent<Factory>();
            _factory.MakeWindowAssociation(_form.Handle, WindowAssociationFlags.IgnoreAll);

            //Temp
            //var _vsByteCode = ShaderBytecode.Compile(Properties.Resources.BasicShader.SubArray(3), "VS", "vs_4_0");
            //_vertexShader = new VertexShader(_device, _vsByteCode);

            //var _psByteCode = ShaderBytecode.Compile(Properties.Resources.BasicShader.SubArray(3), "PS", "ps_4_0");
            //_pixelShader = new PixelShader(_device, _psByteCode);

            //_signature = ShaderSignature.GetInputSignature(_vsByteCode);

            //_layout = new InputLayout(_device, _signature, new InputElement[]
            //{
            //    new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
            //    new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
            //});
            //Temp

            //Temp UI shader
        //    var _vsByteCode = ShaderBytecode.Compile(Properties.Resources.UIShader.SubArray(3), "VS", "vs_4_0");
        //    _vertexShader = new VertexShader(_device, _vsByteCode);

        //    var _psByteCode = ShaderBytecode.Compile(Properties.Resources.UIShader.SubArray(3), "PS", "ps_4_0");
        //    _pixelShader = new PixelShader(_device, _psByteCode);

        //    _signature = ShaderSignature.GetInputSignature(_vsByteCode);

        //    _layout = new InputLayout(_device, _signature, new InputElement[]
        //        {
        //        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
        //        new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
        //});

            //Temp

            //ShaderInformation.LoadAll();

            //foreach (ShaderInformation si in ShaderInformation.ShaderInfoList)
            //{
            //    ShaderBytecode vsByteCode = ShaderBytecode.CompileFromFile(si.Filename, si.VertexShaderFunctionName, "vs_4_0");
            //    _vertexShaderBytecodeList.Add(si.Name, vsByteCode);

            //    VertexShader vs = new VertexShader(_device, vsByteCode);
            //    _vertexShaderList.Add(si.Name, vs);

            //    ShaderBytecode psByteCode = ShaderBytecode.CompileFromFile(si.Filename, si.PixelShaderFunctionName, "ps_4_0");
            //    _pixelShaderBytecodeList.Add(si.Name, psByteCode);

            //    PixelShader ps = new PixelShader(_device, psByteCode);
            //    _pixelShaderList.Add(si.Name, ps);

            //    ShaderSignature sig = ShaderSignature.GetInputSignature(vsByteCode);
            //    _vertexShaderSignatureList.Add(si.Name, sig);


            //    InputElement[] elements = new InputElement[si.InputElementCount];

            //    for (int i = 0; i < si.InputElementCount; i++)
            //    {
            //        elements[i] = new InputElement();
            //    }


            //    InputLayout layout = new InputLayout(_device, vsByteCode, elements);
            //}

            //Defines a constant buffer holding the WorldViewProj for use by the VertexShader
            _constantBuffer = new Buffer(_device, SharpDX.Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            //_context.InputAssembler.InputLayout = _layout;
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            //_context.VertexShader.Set(_vertexShader);
            //Constant buffer containing the world-view-projection matrix
            _context.VertexShader.SetConstantBuffer(0, _constantBuffer);
            //_context.PixelShader.Set(_pixelShader);

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

            _context.OutputMerger.SetTargets(_depthView, _renderTargetView);

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

        public void DrawMesh(Mesh m)
        {
            _meshList.Add(m);
        }

        internal void Draw()
        {
            _cameraView = Matrix.LookAtLH(CameraPosition, CameraTarget, CameraUnitUp);
            _cameraProj = Matrix.PerspectiveFovLH((float)(Math.PI / 4.0f), (float)(_form.ClientSize.Width / _form.ClientSize.Height), 1f, 1000f);
            _cameraWorld = Matrix.Identity;
            _worldViewProj = _cameraWorld * _cameraView * _cameraProj;

            _worldViewProj.Transpose();
            _context.UpdateSubresource(ref _worldViewProj, _constantBuffer);

            _context.ClearRenderTargetView(_renderTargetView, BackgroundColor);
            _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            //Vector4[] verts = new Vector4[]
            //{
            //    new Vector4(0,0,0,0),new Vector4(1,1,1,1),
            //    new Vector4(0,1,0,0),new Vector4(1,1,1,1),
            //    new Vector4(1,1,0,0),new Vector4(1,1,1,1)
            //};

            //Buffer _vBuffer = Buffer.Create(_device, BindFlags.VertexBuffer, verts);
            //_context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vBuffer, 32, 0));
            //_context.Draw(3, 0);

            //_vBuffer.Dispose();

            foreach (Mesh m in _meshList)
            {
                if (m.Material == null)
                    continue;

                _context.VertexShader.Set(m.Material.Shader.VertexShader);
                _context.PixelShader.Set(m.Material.Shader.PixelShader);
                _context.InputAssembler.InputLayout = m.Material.Shader.InputLayout;

                #region Transformation
                Matrix worldMatrix = Matrix.Identity;

                Matrix rotX = Matrix.RotationX(m.Transform.Rotation.X);
                Matrix rotY = Matrix.RotationY(m.Transform.Rotation.Y);
                Matrix rotZ = Matrix.RotationZ(m.Transform.Rotation.Z);

                Matrix translation = Matrix.Translation(m.Transform.WorldPosition);
                Matrix scale = Matrix.Scaling(m.Transform.Scale);

                worldMatrix = translation * (rotX * rotY * rotZ) * scale;

                _worldViewProj = worldMatrix * _cameraView * _cameraProj;
                _worldViewProj.Transpose();

                _context.UpdateSubresource(ref _worldViewProj, _constantBuffer);
                _context.VertexShader.SetConstantBuffer(0, _constantBuffer);
                #endregion                

                Buffer _vertexBuffer = Buffer.Create(_device, BindFlags.VertexBuffer, m.InputElements);
                Buffer _indexBuffer = Buffer.Create(_device, BindFlags.IndexBuffer, m.Triangles);

                _context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, 32, 0));
                _context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);

                _context.DrawIndexed(m.Triangles.Length, 0, 0);

                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();
            }
            _swapChain.Present(0, PresentFlags.None);

            _meshList.Clear();
        }

        internal void Start()
        {
            RenderLoop.Run(_form, _parentEngine.EngineLoop);
        }



    }
}
