using RealTimeCharts.Domain.Models;
using RealTimeCharts.Microservices.DataProvider.Interfaces;
using System;

namespace RealTimeCharts.Microservices.DataProvider.Services
{
    public class DataGenerator : IDataGenerator
    {
        public DataPoint GenerateHeartData(double name)
        {
            var angle = Math.PI * name / 180.0;
            var value = Math.Round(Convert.ToDouble(3 - 1.5 * Math.Sin(angle) + Math.Cos(2 * angle) - 1.5 * Math.Abs(Math.Cos(angle))), 3);
            return new(name, value);
        }
    }
}
