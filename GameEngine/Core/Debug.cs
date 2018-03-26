using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public static class Debug
    {
        public static void Update()
        {

        }

        public static void WriteLine(object Message)
        {
            Console.WriteLine(Message.ToString());
        }

        public delegate void BenchmarkAction();
        public static TimeSpan Benchmark(BenchmarkAction Action)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Action();
            sw.Stop();
            return sw.Elapsed;
        }

    }
}
