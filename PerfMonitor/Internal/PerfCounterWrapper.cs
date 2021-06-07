using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfMonitor.Internal
{
    class PerfCounterWrapper : IPerfCounter
    {

        private PerformanceCounter _counter;
        private string _name;

        public PerfCounterWrapper(PerformanceCounter counter)
        {
            _counter = counter;
            _name = counter.CounterName;
        }

        public string CounterName
        {
            get
            {
                return _name;
            }
        }

        public bool TryGetNextValue(out float value)
        {
            try
            {
                if (_counter.CounterName == "% Privileged Time" || _counter.CounterName == "% User Time" || _counter.CounterName == "% Processor Time")
                {
                    // special case
                    value = (_counter.NextValue() / Environment.ProcessorCount);
                }
                else
                {
                    value = _counter.NextValue();
                }
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }

        public void Close()
        {
            try
            {
                _counter.Close();
            }
            catch { }
        }
    }
}
