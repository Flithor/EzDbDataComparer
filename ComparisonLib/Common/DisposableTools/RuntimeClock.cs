using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ComparisonLib.Common.DisposableTools
{
    public class RuntimeClock : IDisposable
    {
        Stopwatch _sw = new Stopwatch();
        public RuntimeClock()
        {
            Console.WriteLine("Clock Start!");
            _sw.Start();
        }
        public void Dispose()
        {
            _sw.Stop();
            Console.WriteLine("Clock Stop!");
            Console.WriteLine($"Time Spent: {_sw.Elapsed}");
        }
    }
}
