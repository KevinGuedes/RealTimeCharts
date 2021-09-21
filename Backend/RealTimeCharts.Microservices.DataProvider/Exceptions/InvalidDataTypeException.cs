using System;

namespace RealTimeCharts.Microservices.DataProvider.Exceptions
{
    public class InvalidDataTypeException : Exception
    {
        public InvalidDataTypeException(string message) : base(message)
        {
        }
    }
}
