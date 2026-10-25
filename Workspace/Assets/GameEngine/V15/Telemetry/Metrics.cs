using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.V15.Telemetry
{
    public sealed class Metrics
    {
        private readonly ConcurrentDictionary<string, long> counters = new();
        private readonly ConcurrentDictionary<string, double> gauges = new();
        public void Increment(string name, long value = 1) => counters.AddOrUpdate(name, value, (_, current) => current + value);
        public void SetGauge(string name, double value) => gauges[name] = value;
        public long Counter(string name) => counters.TryGetValue(name, out var value) ? value : 0;
        public double Gauge(string name) => gauges.TryGetValue(name, out var value) ? value : 0;
        public IReadOnlyDictionary<string, long> Counters => counters.ToDictionary(x => x.Key, x => x.Value);
    }
}
