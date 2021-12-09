using Newtonsoft.Json;
using RealTimeCharts.Shared.Exceptions;
using System;

namespace RealTimeCharts.Shared.Models
{
    public class DataPoint
    {
        public DataPoint(double name, double value)
        {
            ValidateDomain(name, value);

            Name = Math.Round(name, 3);
            Value = Math.Round(value, 3);
        }

        public double Name { get; }
        public double Value { get; }
        public override string ToString() => $"({Name}, {Value})";

        private void ValidateDomain(double name, double value)
            => InvalidDomainException.When(Double.IsNaN(name) || Double.IsNaN(value), "Invalid domain property");
    }
}
