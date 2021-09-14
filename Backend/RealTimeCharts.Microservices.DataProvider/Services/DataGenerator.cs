using RealTimeCharts.Domain.Models;
using RealTimeCharts.Microservices.DataProvider.Domain;
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

        private readonly Dictionary<DataType, OptimalSetup> _optimalSetup = new()
        {
            [DataType.Heart] = new(0, 360, 10),
            [DataType.Polynomial] = new(-2.1, 6.1, 0.4),
            [DataType.Logarithmic] = new(0, 10, 0.2),
        };

        public int GetSleepTimeByGenerationRate(DataGenerationRate rate)
            => _dataGenerationRate[rate];

        public OptimalSetup GetOptimalSetupFor(DataType dataType)
           => _optimalSetup[dataType];

        public DataPoint GenerateData(double name, DataType dataType)
        {
            return dataType switch
            {
                DataType.Heart => GenerateHeartData(name),
                DataType.Polynomial => GeneratePolynomialData(name),
                DataType.Logarithmic => GenerateLogarithmicData(name),
                _ => GenerateHeartData(name),
            };
        }

        private DataPoint GeneratePolynomialData(double name)
            => new(name, Math.Pow(name, 2) - 2 * name + 4);

        private DataPoint GenerateHeartData(double name)
        {
            var angle = Math.PI * name / 180.0;
            var value = Math.Round(Convert.ToDouble(3 - 1.5 * Math.Sin(angle) + Math.Cos(2 * angle) - 1.5 * Math.Abs(Math.Cos(angle))), 3);
            return new(name, value);
        }

        private DataPoint GenerateLogarithmicData(double name)
            => new(name, Math.Pow(name + 1, 2) * Math.Log(name + 1));
    }
}
