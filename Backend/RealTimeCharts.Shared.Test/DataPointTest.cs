using FluentAssertions;
using RealTimeCharts.Shared.Exceptions;
using RealTimeCharts.Shared.Models;
using System;
using Xunit;

namespace RealTimeCharts.Shared.Test
{
    public class DataPointTest
    {
        [Fact]
        public void ShoudlNotThrowException_WhenDataPointIsValid()
        {
            var exception = Record.Exception(() => new DataPoint(1, 1));

            Assert.Null(exception);
        }

        [Fact]
        public void ShoudlThrowException_WhenDataPointIsInvalid()
        {
            Action action = () => new DataPoint(double.NaN, 1);

            action
                .Should()
                .Throw<InvalidDomainException>()
                .WithMessage("Invalid domain property");
        }
    }
}
