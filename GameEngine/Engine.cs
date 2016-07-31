using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class Engine
    {
        internal static bool _finished = false;

        public static bool Finished
        {
            get
            {
                return _finished;
            }
        }

        public static void Init()
        {
            Graphics.Init();
        }

        public static void Exit()
        {
            Dispose();
            _finished = true;
        }

        public static void Update()
        {

        }

        public static void Draw()
        {
            Graphics.Draw();
        }

        internal static void Dispose()
        {
            Graphics.Dispose();
        }
    }
}
