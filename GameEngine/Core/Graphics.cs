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


        public Vector3 CameraPosition = new Vector3(0, 2, -3f);
        public Vector3 CameraTarget = Vector3.Zero;
        internal Vector3 CameraUnitUp = Vector3.UnitY;

        public Matrix WorldViewProj;
        public Matrix CameraView;
        public Matrix CameraProj;

        public RenderForm Form { get { return _form; } }
        public Color BackgroundColor { get; set; } = Color.Gray;
        public Device GraphicsDevice { get { return _device; } }

        public float ZFarDistance { get; set; } = 200f;

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

            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

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

            CameraView = Matrix.LookAtLH(CameraPosition, CameraTarget, CameraUnitUp);
            CameraProj = Matrix.PerspectiveFovLH((float)(Math.PI / 4.0f), (float)(_form.ClientSize.Width / _form.ClientSize.Height), 1f, ZFarDistance);
            WorldViewProj = CameraView * CameraProj;

            _context.ClearRenderTargetView(_renderTargetView, BackgroundColor);
            _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            foreach (Material m in Material._materialList)
                m.DrawAll();

            _swapChain.Present(0, PresentFlags.None);
        }

        //Public helpers
        private Shader _currentShader;

        public void SetShader(Shader shader)
        {
            if (_currentShader == shader)
                return;

            _context.VertexShader.Set(shader.VertexShader);
            _context.PixelShader.Set(shader.PixelShader);
            _context.InputAssembler.InputLayout = shader.InputLayout;
            _currentShader = shader;
        }
        public Buffer CreateVertexBuffer(Vector4[] Data) { return Buffer.Create(_device, BindFlags.VertexBuffer, Data); }
        public Buffer CreateVertexBuffer<T>(T[] Data) where T : struct
        {
            return Buffer.Create(_device, BindFlags.VertexBuffer, Data);
        }
        public Buffer CreateIndexBuffer(int[] Data) { return Buffer.Create(_device, BindFlags.IndexBuffer, Data); }
        public Buffer CreateConstantBuffer(int Size) { return new Buffer(_device, Size, ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0); }
        public void UpdateConstantBuffer<T>(ref T Data, Buffer ConstantBuffer) where T : struct
        {
            _context.UpdateSubresource(ref Data, ConstantBuffer);
        }
        public void UpdateConstantBuffer<T>(T[] Data, Buffer ConstantBuffer) where T :struct
        {
            _context.UpdateSubresource(Data, ConstantBuffer);
        }
        public void SetIndexBuffer(Buffer Buffer) { _context.InputAssembler.SetIndexBuffer(Buffer, Format.R32_UInt, 0); }
        public void SetVertexBuffers(VertexBufferBinding[] Buffers) { _context.InputAssembler.SetVertexBuffers(0, Buffers); }
        public void SetConstantBuffer(int BufferIndex, Buffer ConstantBuffer) { _context.VertexShader.SetConstantBuffer(BufferIndex,ConstantBuffer); }

        //Public Draw calls
        public void DrawIndexed(int IndexCount, int StartIndexLocation, int BaseVertexLocation)
        {
            _context.DrawIndexed(IndexCount, StartIndexLocation, BaseVertexLocation);
        }
        public void DrawIndexedInstanced(int IndexCount, int InstanceCount,int StartIndexLocation, int BaseVertexLocation, int StartInstanceLocation)
        {
            _context.DrawIndexedInstanced(IndexCount,InstanceCount, 0, 0, 0);

        }
    }
}
