using System;
using System.Collections.Generic;

namespace PerfMonitor
{
    class PerfCounterConsoleOutput : IPerfCounterOutput
    {
        private static int KB = 1024;
        private static int MB = 1048576;
        private static int GB = 1073741824;

        public void BeginOutput()
        {
            Console.WriteLine("----------------------------------------------------");
        }

        public void Output(Dictionary<string, float> metrics)
        {
            foreach(var kvp in metrics)
            {
                Console.WriteLine(Format(kvp.Key, kvp.Value));
            }
        }

        public void EndOutput()
        {
            Console.WriteLine("----------------------------------------------------");
        }

        private static string Format(string metric, float value)
        {
            string message = metric + ": ";
            if (metric.StartsWith("%"))
            {
                message += value + " %";
            }
            else if (value > KB && value <= MB)
            {
                message += (value / KB) + " KB";
            }
            else if (value > MB && value <= GB)
            {
                message += (value / MB) + " MB";
            }
            else if (value > GB)
            {
                message += (value / GB) + " GB";
            }
            else
            {
                message += value + " BYTES";
            }
            return message;
        }
    }
}
