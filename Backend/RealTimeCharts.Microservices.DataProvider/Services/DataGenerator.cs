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
            [DataType.Heart] = new(0, 360, 5),
            [DataType.Polynomial] = new(-4, 7, 0.5),
            [DataType.Logarithmic] = new(0, 10, 0.2),
            [DataType.Fibonacci] = new(1, 30, 1),
            [DataType.Weibull] = new(0, 5, 0.1),
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
                DataType.Fibonacci => GenerateFibonacciData(name),
                DataType.Weibull => GenerateWeibullData(name),
                _ => throw new ArgumentException("Invalid Data Type")
            };
        }

        private static DataPoint GeneratePolynomialData(double name)
            => new(name, (-1) * Math.Pow(name, 3) + Math.Pow(name, 2) - 2 * name + 170);

        private static DataPoint GenerateHeartData(double name)
        {
            var angle = Math.PI * name / 180.0;
            var value = Math.Round(Convert.ToDouble(3 - 1.5 * Math.Sin(angle) + Math.Cos(2 * angle) - 1.5 * Math.Abs(Math.Cos(angle))), 3);
            return new(name, value);
        }

        private static DataPoint GenerateLogarithmicData(double name)
            => new(name, Math.Pow(name + 1, 2) * Math.Log(name + 1));

        private static DataPoint GenerateWeibullData(double name)
        {
            double shape = 3;
            double scale = 2;
            return new(name, (shape / shape) * Math.Pow(name / scale, shape - 1) * Math.Pow(Math.E, Math.Pow(-name / scale, shape)));
        }

        private static DataPoint GenerateFibonacciData(double name)
        {
            int a = 0, b = 1, c = 0;
            int n = Convert.ToInt32(name);

            for (int i = 2; i <= n; i++)
            {
                c = a + b;
                a = b;
                b = c;
            }

            return new(n, b);
        }
    }
}
