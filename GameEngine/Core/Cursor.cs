using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;

namespace GameEngine
{
    public static class Cursor
    {
        private static bool _locked = false;
        private static Vector2 _lockPosition;
        public static void Update()
        {
            if (_locked == true)
                Position = _lockPosition;
        }

        public static Vector2 Position
        {
            get { return new Vector2(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y); }
            set { System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)value.X, (int)value.Y); }
        }

        public static void Hide()
        {
            System.Windows.Forms.Cursor.Hide();
        }

        public static void Show()
        {
            System.Windows.Forms.Cursor.Show();
        }

        public static void Lock()
        {
            _locked = true;
            _lockPosition = Position;
        }

        public static void Unlock()
        {
            _locked = false;
        }
    }
}
