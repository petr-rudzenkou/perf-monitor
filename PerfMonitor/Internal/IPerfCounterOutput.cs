using System.Collections.Generic;

namespace PerfMonitor
{
    interface IPerfCounterOutput
    {
        void BeginOutput();

        void Output(Dictionary<string, float> metrics);

        void EndOutput();
    }
}
