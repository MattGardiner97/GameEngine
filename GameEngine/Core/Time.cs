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

        public static float DeltaTime { get; private set; }

        public static void Start()
        {
            _stopwatch.Start();
        }

        public static void Update()
        {
            _stopwatch.Stop();
            DeltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();
            
        }

    }
}
