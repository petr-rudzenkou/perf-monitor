using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfMonitor.Internal
{
    interface IPerfCounter
    {
        string CounterName { get; }

        bool TryGetNextValue(out float value);

        void Close();
    }
}
