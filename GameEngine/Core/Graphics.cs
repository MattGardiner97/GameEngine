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


        internal void Draw()
        {
            CameraPosition = Camera.MainCamera.Transform.WorldPosition;

            
            _cameraView = Matrix.LookAtLH(CameraPosition, CameraTarget, CameraUnitUp);
            _cameraProj = Matrix.PerspectiveFovLH((float)(Math.PI / 4.0f), (float)(_form.ClientSize.Width / _form.ClientSize.Height), 1f, 1000f);

            _context.ClearRenderTargetView(_renderTargetView, BackgroundColor);
            _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            MeshRenderer[] _meshList = GameObject.GetAllComponents<MeshRenderer>();            

            foreach (MeshRenderer mr in _meshList)
            {
                if (mr.Material == null)
                    continue;

                _context.VertexShader.Set(mr.Material.Shader.VertexShader);
                _context.PixelShader.Set(mr.Material.Shader.PixelShader);
                _context.InputAssembler.InputLayout = mr.Material.Shader.InputLayout;

                #region Transformation
                Matrix worldMatrix = Matrix.Identity;

                Matrix rotX = Matrix.RotationX(mr.Transform.Rotation.X);
                Matrix rotY = Matrix.RotationY(mr.Transform.Rotation.Y);
                Matrix rotZ = Matrix.RotationZ(mr.Transform.Rotation.Z);

                Matrix translation = Matrix.Translation(mr.Transform.WorldPosition);
                Matrix scale = Matrix.Scaling(mr.Transform.Scale);

                worldMatrix = translation * (rotX * rotY * rotZ) * scale;

                _worldViewProj = worldMatrix * _cameraView * _cameraProj;
                _worldViewProj.Transpose();

                _context.UpdateSubresource(ref _worldViewProj, _constantBuffer);
                #endregion                

                Buffer _vertexBuffer = Buffer.Create(_device, BindFlags.VertexBuffer, mr.InputElements);
                Buffer _indexBuffer = Buffer.Create(_device, BindFlags.IndexBuffer, mr.Mesh.Triangles);

                _context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, 32, 0));
                _context.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);

                _context.DrawIndexed(mr.Mesh.Triangles.Length, 0, 0);

                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();
            }
            _swapChain.Present(0, PresentFlags.None);
        }

        internal void Start()
        {
            RenderLoop.Run(_form, _parentEngine.EngineLoop);
        }



    }
}
