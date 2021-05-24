using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PerfMonitor
{
    class PerfCounterRunner : IPerfCounterRunner
    {
        private readonly IConfiguration _config;
        private readonly IPerfCounterProvider _counterProvider;
        private readonly IPerfCounterOutput _output;

        public PerfCounterRunner(IConfiguration config, IPerfCounterProvider counterProvider, IPerfCounterOutput output)
        {
            _config = config;
            _counterProvider = counterProvider;
            _output = output;
        }

        public void Run()
        {
            var processName = _config.GetValue<string>("PerfMonitor:ProcessName");
            var duration = _config.GetValue<int>("PerfMonitor:Duration");
            var interval = _config.GetValue<int>("PerfMonitor:Interval");
            var counterNames = _config.GetSection("PerfMonitor:Counters").Get<string[]>();
            var iterations = duration / interval;
            interval = interval * 1000;

            var counters = counterNames.SelectMany(counterName => _counterProvider.GetPerfCounters(processName, counterName)).ToList();
            foreach (var counter in counters)
            {
                counter.NextValue();
            }

            Thread.Sleep(interval);

            var allMetrics = new List<Dictionary<string, float>>();
            for (int i = 0; i < iterations; ++i)
            {
                Dictionary<string, float> metrics = new Dictionary<string, float>();
                foreach (var counter in counters)
                {
                    float metric;
                    if (counter.CounterName == "% Privileged Time" || counter.CounterName == "% User Time" || counter.CounterName == "% Processor Time")
                    {
                        // special case
                        metric = counter.NextValue() / Environment.ProcessorCount;
                    }
                    else
                    {
                        metric = counter.NextValue();
                    }

                    float currentMetric;
                    if (metrics.TryGetValue(counter.CounterName, out currentMetric))
                    {
                        metrics[counter.CounterName] = currentMetric + metric;
                    }
                    else
                    {
                        metrics.Add(counter.CounterName, metric);
                    }
                }

                allMetrics.Add(metrics);

                var average = allMetrics.SelectMany(dict => dict)
                         .ToLookup(pair => pair.Key, pair => pair.Value)
                         .ToDictionary(group => $"{group.Key} (av)", group => group.Average());

                _output.BeginOutput();
                _output.Output(metrics);
                _output.Output(average);
                _output.EndOutput();

                Thread.Sleep(interval);
            }

            Console.WriteLine("Any key to exit...");
            Console.Read();
        }
    }
}
