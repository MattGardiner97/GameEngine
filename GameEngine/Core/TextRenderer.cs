using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.DirectWrite;

using Color = SharpDX.Color;
using GameEngine.Structures;

namespace GameEngine.Core
{
    public class TextRenderer
    {
        private SharpDX.Direct2D1.Factory _factory2D = new SharpDX.Direct2D1.Factory();
        private SharpDX.DirectWrite.Factory _writeFactory = new SharpDX.DirectWrite.Factory();
        private Surface _surface;
        private PixelFormat _pixelFormat;
        private RenderTargetProperties _renderTargetProperties;
        private RenderTarget _renderTarget;

        private static Dictionary<Color, SolidColorBrush> _brushCache = new Dictionary<Color, SolidColorBrush>();


        public void Init(Texture2D _backBuffer)
        {
            _surface = _backBuffer.QueryInterface<Surface>();
            _pixelFormat = new PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied);
            _renderTargetProperties = new RenderTargetProperties(_pixelFormat);
            _renderTarget = new RenderTarget(_factory2D, _surface, _renderTargetProperties);
            _renderTarget.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Cleartype;
        }

        public void DrawTextRenderObject(TextRenderObject RenderObject)
        {
            if (RenderObject.Visible == false)
                return;

            TextFormat tFormat = GetTextFormat(RenderObject);
            TextLayout tLayout = GetTextLayout(RenderObject,tFormat);
            SolidColorBrush brush = GetBrush(RenderObject.TextColor);


            _renderTarget.BeginDraw();
            _renderTarget.DrawTextLayout(RenderObject.ScreenPosition, tLayout, brush);
            _renderTarget.EndDraw();
        }

        private TextFormat GetTextFormat(TextRenderObject tro)
        {
            if (tro._recreate == true)
            {
                if (tro._textFormat != null)
                    tro._textFormat.Dispose();

                TextFormat tFormat = new TextFormat(_writeFactory, tro.FontName, tro.FontSize)
                {
                    TextAlignment = tro.TextAlignment,
                    ParagraphAlignment = tro.ParagraphAlignment
                };

                tro._textFormat = tFormat;
                return tFormat;
            }
            else
                return tro._textFormat;
        }
        private TextLayout GetTextLayout(TextRenderObject tro,TextFormat tFormat)
        {
            if (tro._recreate == true)
            {
                if (tro._textLayout != null)
                    tro._textLayout.Dispose();

                TextLayout tLayout = new TextLayout(_writeFactory, tro.Text, tFormat, tro.MaxWidth, tro.MaxHeight);
                tro._textLayout = tLayout;
                tro._recreate = false;
                return tLayout;
            }
            else
                return tro._textLayout;
        }
        private SolidColorBrush GetBrush(Color c)
        {
            if (_brushCache.ContainsKey(c) == false)
            {
                SolidColorBrush b = new SolidColorBrush(_renderTarget, c);
                _brushCache.Add(c, b);
                return b;
            }
            else
                return _brushCache[c];
        }

        public void Dispose()
        {
            _factory2D.Dispose();
            _writeFactory.Dispose();
            _surface.Dispose();
            _renderTarget.Dispose();

            foreach(TextRenderObject tro in TextRenderObject._objectList)
            {
                tro._textFormat.Dispose();
                tro._textLayout.Dispose();
            }
        }

    }
}
