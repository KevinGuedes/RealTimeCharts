using PaymentContext.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeCharts.Domain.Events
{
    public class GenerateHeartDataEvent : Event
    {
        public GenerateHeartDataEvent(int dataPoints)
        {
            DataPoints = dataPoints;
        }

        public int DataPoints { get; set; }
    }
}
