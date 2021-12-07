using RealTimeCharts.Microservices.DataProvider.Services;
using RealTimeCharts.Shared.Enums;
using RealTimeCharts.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using RealTimeCharts.Microservices.DataProvider.Exceptions;
using RealTimeCharts.Microservices.DataProvider.Domain;

namespace RealTimeCharts.Microservices.DataProvider.Test.Services
{
    public class DataGeneratorTest
    {
        private readonly DataGenerator _sut;
        public DataGeneratorTest()
            => _sut = new();

        public static IEnumerable<object[]> ValidDataTypes()
        {
            var validDataTypes = new List<object[]>();

            foreach (int i in Enum.GetValues(typeof(DataType)))
            {
                if ((DataType)i == DataType.Invalid)
                    continue;

                validDataTypes.Add(new object[] { i });
            }

            return validDataTypes;
        }

        public static IEnumerable<object[]> DataTypes()
        {
            var dataTypes = new List<object[]>();

            foreach (int i in Enum.GetValues(typeof(DataType)))
                dataTypes.Add(new object[] { i });

            return dataTypes;
        }

        public static IEnumerable<object[]> DataGenerationRates()
        {
            var dataGenerationRates = new List<object[]>();

            foreach (int i in Enum.GetValues(typeof(DataGenerationRate)))
                dataGenerationRates.Add(new object[] { i });

            return dataGenerationRates;
        }

        [Theory]
        [MemberData(nameof(ValidDataTypes))]
        public void ShouldNotThrowException_WhenValidDataTypesAreUsed(DataType dataType)
        {
            var exception = Record.Exception(() => _sut.GenerateData(1, dataType));

            Assert.Null(exception);
        }

        [Fact]
        public void ShouldThrowException_WhenDataTypeInvalidIsUsed()
        {
            Action action = () => _sut.GenerateData(1, DataType.Invalid);

            action
                .Should()
                .Throw<InvalidDomainException>()
                .WithMessage("Invalid domain property");
        }

        [Fact]
        public void ShouldThrowException_WhenInvalidDataTypeIsUsed()
        {
            Action action = () => _sut.GenerateData(1, (DataType)(-1));

            action
                .Should()
                .Throw<InvalidDataTypeException>()
                .WithMessage("Invalid Data Type");
        }

        [Theory]
        [MemberData(nameof(DataTypes))]
        public void ShouldReturnOptimalSetup_ForEachDataType(DataType dataType)
        {
            var result = _sut.GetOptimalSetupFor(dataType);

            Assert.IsType<OptimalSetup>(result);
        }

        [Theory]
        [MemberData(nameof(DataGenerationRates))]
        public void ShouldReturnSleepTime_ForEachDataGenerationRate(DataGenerationRate dataGenerationRate)
        {
            var result = _sut.GetSleepTimeByGenerationRate(dataGenerationRate);

            Assert.IsType<int>(result);
        }
    }
}
