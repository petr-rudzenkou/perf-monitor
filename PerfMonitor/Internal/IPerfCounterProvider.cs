using System.Collections.Generic;
using System.Diagnostics;

namespace PerfMonitor
{
    interface IPerfCounterProvider
    {
        PerformanceCounter GetPerfCounter(int processId, string processCounterName);

        IEnumerable<PerformanceCounter> GetPerfCounters(string processName, string processCounterName);
    }
}
