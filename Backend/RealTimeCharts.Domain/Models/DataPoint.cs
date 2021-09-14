using System;

namespace RealTimeCharts.Domain.Models
{
    public class DataPoint
    {
        public DataPoint(double name, double value)
        {
            Name = Math.Round(name, 3);
            Value = Math.Round(value, 3);
        }

        public double Name { get; }
        public double Value { get; }

        public override string ToString() => $"({Name}, {Value})";
    }
}
