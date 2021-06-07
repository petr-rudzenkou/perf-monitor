using PerfMonitor.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PerfMonitor
{
    internal class PerfCounterProvider : IPerfCounterProvider
    {
        public IPerfCounter GetPerfCounter(int processId, string processCounterName)
        {
            string instance = GetInstanceNameForProcessId(processId);
            if (string.IsNullOrEmpty(instance))
                return null;

            return new PerfCounterWrapper(new PerformanceCounter("Process", processCounterName, instance));
        }

        public IEnumerable<IPerfCounter> GetPerfCounters(string processName, string processCounterName)
        {
            return new PerformanceCounterCategory("Process").GetInstanceNames()
                .Where(name => name.ToUpper().StartsWith(processName.ToUpper()))
                .Select(name => new PerfCounterWrapper(new PerformanceCounter("Process", processCounterName, name)));
        }

        private static string GetInstanceNameForProcessId(int processId)
        {
            using (var process = Process.GetProcessById(processId))
            {
                string processName = Path.GetFileNameWithoutExtension(process.ProcessName);

                PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
                string[] instances = cat.GetInstanceNames()
                    .Where(inst => inst.StartsWith(processName))
                    .ToArray();

                foreach (string instance in instances)
                {
                    using (PerformanceCounter cnt = new PerformanceCounter("Process", "ID Process", instance, true))
                    {
                        int val = (int)cnt.RawValue;
                        if (val == processId)
                        {
                            return instance;
                        }
                    }
                }
                return null;
            }
        }
    }
}
