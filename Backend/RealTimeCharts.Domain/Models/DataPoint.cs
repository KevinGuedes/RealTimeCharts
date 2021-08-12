namespace RealTimeCharts.Domain.Models
{
    public class DataPoint
    {
        public DataPoint(double name, double value)
        {
            Name = name;
            Value = value;
        }

        public double Name { get; }
        public double Value { get; }

        public override string ToString() => $"({Name}, {Value})";
    }
}
