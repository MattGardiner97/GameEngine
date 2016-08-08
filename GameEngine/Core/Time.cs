using System;
using System.Collections.Generic;
using System.Linq;
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
            _stopwatch.Stop();
            _deltaTime = _stopwatch.ElapsedMilliseconds / 1000f;
            _stopwatch.Reset();
            _stopwatch.Start();
            
        }

    }
}
