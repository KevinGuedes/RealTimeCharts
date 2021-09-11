using System;

namespace RealTimeCharts.Infra.Bus.Exceptions
{
    public class NoConnectionEstablishedException : Exception
    {
        public NoConnectionEstablishedException(string message) : base(message)
        {
        }
    }
}
