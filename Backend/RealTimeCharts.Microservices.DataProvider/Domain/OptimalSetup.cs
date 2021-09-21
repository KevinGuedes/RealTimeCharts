namespace RealTimeCharts.Microservices.DataProvider.Domain
{
    public class OptimalSetup
    {
        public OptimalSetup(double min, double max, double step)
        {
            Min = min;
            Max = max;
            Step = step;
        }

        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }
    }
}
