using RealTimeCharts.Domain.Models;
using RealTimeCharts.Microservices.DataProvider.Interfaces;
using RealTimeCharts.Shared.Enums;
using System;
using System.Collections.Generic;

namespace RealTimeCharts.Microservices.DataProvider.Services
{
    public class DataGenerator : IDataGenerator
    {
        private readonly Dictionary<DataGenerationRate, int> _dataGenerationRate = new()
        {
            [DataGenerationRate.Ultra] = 300,
            [DataGenerationRate.High] = 600,
            [DataGenerationRate.Medium] = 800,
            [DataGenerationRate.Low] = 1000
        };

        public int GetSleepTimeByGenerationRate(DataGenerationRate rate)
            => _dataGenerationRate[rate];

        public DataPoint GenerateHeartData(double name)
        {
            var angle = Math.PI * name / 180.0;
            var value = Math.Round(Convert.ToDouble(3 - 1.5 * Math.Sin(angle) + Math.Cos(2 * angle) - 1.5 * Math.Abs(Math.Cos(angle))), 3);
            return new(name, value);
        }
    }
}
