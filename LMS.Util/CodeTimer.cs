using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace RFD.FMS.Util
{
    public class CodeTimer
    {
        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Time("", 1, () => { });
        }

        public static string Time(string name, Action action)
        {
            return Time(name, 1, action);
        }

        public static string Time(string name, int iteration, Action action)
        {
            if (String.IsNullOrEmpty(name)) return string.Empty;

            var resultStr = new StringBuilder();
            // 1.
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);
            resultStr.AppendLine(name);

            // 2.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            // 3.
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ulong cycleCount = GetCycleCount();
            for (int i = 0; i < iteration; i++) action();
            ulong cpuCycles = GetCycleCount() - cycleCount;
            watch.Stop();

            // 4.
            Console.ForegroundColor = currentForeColor;
            var timeElapsedStr = "\t时间开销:\t" + watch.ElapsedMilliseconds.ToString("N0") + "ms";
            Console.WriteLine(timeElapsedStr);
            resultStr.AppendLine(timeElapsedStr);

            var cpuCyclesStr = "\tCPU周期:\t" + cpuCycles.ToString("N0");
            //var cpuCyclesStr = "\tCPU Cycles:\t" + cpuCycles.ToString("N0");
            Console.WriteLine(cpuCyclesStr);
            resultStr.AppendLine(cpuCyclesStr);

            // 5.
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine("\tGen " + i + ": \t\t" + count);
                //resultStr.AppendLine("\tGen " + i + ": \t\t" + count);
            }

            Console.WriteLine();
            return resultStr.ToString();
        }

        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();
    }
}
