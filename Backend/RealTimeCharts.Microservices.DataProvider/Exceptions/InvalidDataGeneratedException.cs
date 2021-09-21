using System;

namespace RealTimeCharts.Microservices.DataProvider.Exceptions
{
    public class InvalidDataGeneratedException : Exception
    {
        public InvalidDataGeneratedException(string message) : base(message)
        {
        }
    }
}
