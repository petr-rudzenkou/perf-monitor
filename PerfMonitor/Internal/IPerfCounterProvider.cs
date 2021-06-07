using PerfMonitor.Internal;
using System.Collections.Generic;

namespace PerfMonitor
{
    interface IPerfCounterProvider
    {
        IPerfCounter GetPerfCounter(int processId, string processCounterName);

        IEnumerable<IPerfCounter> GetPerfCounters(string processName, string processCounterName);
    }
}
