using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DirectWrite;

namespace GameEngine
{
    public class TextRenderObject : Component
    {
        internal static List<TextRenderObject> _objectList = new List<TextRenderObject>();

        private string _text = "";
        private Color _textColor = Color.White;
        private string _fontName = "Arial";
        private int _fontSize = 12;
        private int _maxWidth = Graphics.Current.Form.ClientSize.Width;
        private int _maxHeight = Graphics.Current.Form.ClientSize.Height;
        private TextAlignment _textAlignment = TextAlignment.Center;
        private ParagraphAlignment _paragraphAlignment = ParagraphAlignment.Center;

        public bool Visible { get; set; } = true;
        public string Text { get { return _text; } set { _text = value; _recreate = true; } }
        public Color TextColor { get { return _textColor; } set { _textColor = value; _recreate = true; } }
        public string FontName { get { return _fontName; } set { _fontName = value; _recreate = true; } }
        public int FontSize { get { return _fontSize; } set { _fontSize = value; _recreate = true; } }
        public Vector2 ScreenPosition { get; set; } = Vector2.Zero;
        public int MaxWidth { get { return _maxWidth; } set { _maxWidth = value; _recreate = true; } }
        public int MaxHeight { get { return _maxHeight; } set { _maxHeight = value; _recreate = true; } }
        public TextAlignment TextAlignment { get { return _textAlignment; }set{ _textAlignment = value;_recreate = true; } }
        public ParagraphAlignment ParagraphAlignment { get { return _paragraphAlignment; } set { _paragraphAlignment = value;_recreate = true; } }

        internal bool _recreate = true;
        internal TextFormat _textFormat;
        internal TextLayout _textLayout;


        public TextRenderObject()
        {
            _objectList.Add(this);
        }
    }
}
