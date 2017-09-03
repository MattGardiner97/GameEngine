using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace GameEngine
{
    public static class Time
    {
        private static Stopwatch _stopwatch = new Stopwatch();

        private static float _deltaTime;
        public static float DeltaTime
        {
            get
            {
                return _deltaTime;
            }
        }

        public static void Start()
        {
            _stopwatch.Start();
        }

        public static void WorkMethod()
        {

        }

        public static void Update()
        {
            //1 second = 10000000 ticks

            _stopwatch.Stop();
            _deltaTime = _stopwatch.ElapsedTicks / 10000000f;
            _stopwatch.Restart();
            
        }

    }
}
