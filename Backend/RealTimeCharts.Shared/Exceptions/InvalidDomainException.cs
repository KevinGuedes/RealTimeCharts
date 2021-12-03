using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeCharts.Shared.Exceptions
{
    public class InvalidDomainException : Exception
    {
        public InvalidDomainException(string error) : base(error)
        {
        }

        public static void When(bool hasError, string error)
        {
            if (hasError)
                throw new InvalidDomainException(error);
        }
    }
}
