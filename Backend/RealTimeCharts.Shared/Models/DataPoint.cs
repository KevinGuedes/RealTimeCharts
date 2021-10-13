using System;

namespace RealTimeCharts.Shared.Models
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
        public bool IsValid { get => !Double.IsNaN(Name) && !Double.IsNaN(Value); }
        public override string ToString() => $"({Name}, {Value})";
    }
}
