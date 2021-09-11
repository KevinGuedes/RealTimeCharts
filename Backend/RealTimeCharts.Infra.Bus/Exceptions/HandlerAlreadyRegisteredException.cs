using System;

namespace RealTimeCharts.Infra.Bus.Exceptions
{
    public class HandlerAlreadyRegisteredException : Exception
    {
        public HandlerAlreadyRegisteredException(string message) : base(message) 
        {
        }
    }
}
