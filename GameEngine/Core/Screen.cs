using GameEngine.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Core
{
    public static class Screen
    {
        static Screen()
        {
            Graphics.Current.Form.Resize += (o, args) =>
            {
                Width = Graphics.Current.Form.ClientSize.Width;
                Height = Graphics.Current.Form.ClientSize.Height;
            };
        }

        public static int Width { get; private set; }
        public static int Height { get; private set; }

        //public static Ray ScreenPointToRay(int X, int Y)
        //{
        //    return null;
        //}

    }
}
