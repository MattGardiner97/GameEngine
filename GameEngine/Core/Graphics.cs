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

        //private static InputLayout _layout;
        //private static ShaderSignature _signature;

        private static Dictionary<string, ShaderBytecode> _vertexShaderBytecodeList = new Dictionary<string, ShaderBytecode>();
        private static Dictionary<string, VertexShader> _vertexShaderList = new Dictionary<string, VertexShader>();
        private static Dictionary<string, ShaderSignature> _vertexShaderSignatureList = new Dictionary<string, ShaderSignature>();
        private static Dictionary<string, InputLayout> _vertexShaderLayoutList = new Dictionary<string, InputLayout>();

        private static Dictionary<string, ShaderBytecode> _pixelShaderBytecodeList = new Dictionary<string, ShaderBytecode>();
        private static Dictionary<string, PixelShader> _pixelShaderList = new Dictionary<string, PixelShader>();

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

            //_layout.Dispose();
            //_vsByteCode.Dispose();
            //_vertexShader.Dispose();
            //_psByteCode.Dispose();
            //_pixelShader.Dispose();
            foreach(ShaderBytecode sbc in _vertexShaderBytecodeList.Values)
            {
                sbc.Dispose();
            }
            foreach(ShaderBytecode sbc in _pixelShaderBytecodeList.Values)
            {
                sbc.Dispose();
            }
            foreach(VertexShader vs in _vertexShaderList.Values)
            {
                vs.Dispose();
            }
            foreach(PixelShader ps in _pixelShaderList.Values)
            {
                ps.Dispose();
            }
            foreach(ShaderSignature ss in _vertexShaderSignatureList.Values)
            {
                ss.Dispose();
            }
            foreach(InputLayout il in _vertexShaderLayoutList.Values)
            {
                il.Dispose();
            }

            //_signature.Dispose();

            //_vertexBuffer.Dispose();
            //_indexBuffer.Dispose();
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

            //_vsByteCode = ShaderBytecode.CompileFromFile("Shader.hlsl","VS","vs_4_0");
            //_vertexShader = new VertexShader(_device, _vsByteCode);

            //_psByteCode = ShaderBytecode.CompileFromFile("Shader.hlsl", "PS", "ps_4_0");
            //_pixelShader = new PixelShader(_device, _psByteCode);

            //_signature = ShaderSignature.GetInputSignature(_vsByteCode);

            //_layout = new InputLayout(_device, _signature, new InputElement[]
            //{
            //    new InputElement("POSITION",0,Format.R32G32B32A32_Float,0,0),
            //    new InputElement("COLOR",0,Format.R32G32B32A32_Float,16,0)
            //});

            ShaderInformation.LoadAll();

            foreach(ShaderInformation si in ShaderInformation.ShaderInfoList)
            {
                ShaderBytecode vsByteCode = ShaderBytecode.CompileFromFile(si.Filename,si.VertexShaderFunctionName,"vs_4_0");
                _vertexShaderBytecodeList.Add(si.Name, vsByteCode);

                VertexShader vs = new VertexShader(_device, vsByteCode);
                _vertexShaderList.Add(si.Name,vs);

                ShaderBytecode psByteCode = ShaderBytecode.CompileFromFile(si.Filename, si.PixelShaderFunctionName, "ps_4_0");
                _pixelShaderBytecodeList.Add(si.Name, psByteCode);

                PixelShader ps = new PixelShader(_device, psByteCode);
                _pixelShaderList.Add(si.Name, ps);

                ShaderSignature sig = ShaderSignature.GetInputSignature(vsByteCode);
                _vertexShaderSignatureList.Add(si.Name, sig);


                InputElement[] elements = new InputElement[si.InputElementCount];

                for(int i = 0; i <si.InputElementCount;i++)
                {
                    elements[i] = new InputElement();
                }


                InputLayout layout = new InputLayout(_device, vsByteCode,elements);
            }

            //Defines a constant buffer holding the WorldViewProj for use by the VertexShader
            _constantBuffer = new Buffer(_device, SharpDX.Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

            //_context.InputAssembler.InputLayout = _layout;
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            //_context.VertexShader.Set(_vertexShader);
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

        private static List<Mesh> _meshList = new List<Mesh>();

        public static void DrawMesh(Mesh m)
        {
            _meshList.Add(m);
        }

        internal static void Draw()
        {
            _cameraView = Matrix.LookAtLH(CameraPosition, CameraTarget, CameraUnitUp);
            _cameraProj = Matrix.PerspectiveFovLH((float)(Math.PI / 4.0f), (float)(_form.ClientSize.Width / _form.ClientSize.Height), 1f, 1000f);
            _cameraWorld = Matrix.Identity;
            _worldViewProj = _cameraWorld * _cameraView * _cameraProj;

            _worldViewProj.Transpose();
            _context.UpdateSubresource(ref _worldViewProj, _constantBuffer);

            _context.ClearRenderTargetView(_renderTargetView, BackgroundColor);
            _context.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            foreach (Mesh m in _meshList)
            {
                #region Transformation
                Matrix worldMatrix = Matrix.Identity;

                Matrix rotX = Matrix.RotationX(m.Transform.Rotation.X);
                Matrix rotY = Matrix.RotationY(m.Transform.Rotation.Y);
                Matrix rotZ = Matrix.RotationZ(m.Transform.Rotation.Z);
                
                Matrix translation = Matrix.Translation(m.Transform.WorldPosition);
                Matrix scale = Matrix.Scaling(m.Transform.Scale);

                worldMatrix = translation * (rotX * rotY*rotZ)* scale;

                _worldViewProj = worldMatrix * _cameraView * _cameraProj;
                _worldViewProj.Transpose();
                
                _context.UpdateSubresource(ref _worldViewProj, _constantBuffer);
                _context.VertexShader.SetConstantBuffer(0, _constantBuffer);
                #endregion


                

                Buffer _vertexBuffer = Buffer.Create(_device, BindFlags.VertexBuffer, elementArray);
                Buffer _indexBuffer = Buffer.Create(_device, BindFlags.IndexBuffer, m.Triangles);

                _context.InputAssembler.SetVertexBuffers(0,new VertexBufferBinding(_vertexBuffer,32,0));
                _context.InputAssembler.SetIndexBuffer(_indexBuffer,Format.R32_UInt,0);

                _context.DrawIndexed(m.Triangles.Length, 0, 0);

                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();
            }
            _swapChain.Present(0, PresentFlags.None);

            _meshList.Clear();
        }

        internal static void Start()
        {
            RenderLoop.Run(_form, Engine.EngineLoop);
        }



    }
}
