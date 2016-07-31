using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

using System.Windows.Forms;

using Device = SharpDX.Direct3D11.Device;


namespace GameEngine
{
    public static class Graphics
    {
        internal static RenderForm _form;
        private static SwapChainDescription _swapChainDescription;
        private static Device _device;
        private static SwapChain _swapChain;
        private static DeviceContext _context;
        private static Factory _factory;

        private static RenderTargetView _renderTargetView;
        private static Texture2D _backBuffer;
        private static Texture2D _depthBuffer;
        private static DepthStencilView _depthView;

        internal static RawColor4 _backgroundColor;

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

            

            //Called when the program is exiting
            _form.FormClosing += (sender, args) =>
            {
                Engine.Exit();
            };

            _form.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                {
                    _form.Close();
                }
            };
        }

        internal static void Draw()
        {
            _backgroundColor = new RawColor4(255, 0, 0, 255);

            _context.ClearRenderTargetView(_renderTargetView, _backgroundColor);
        }

        internal static void Dispose()
        {
            _form.Dispose();
            _device.Dispose();
            _swapChain.Dispose();
            _context.Dispose();
            _factory.Dispose();
        }

    }
}
