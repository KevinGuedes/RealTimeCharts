using RealTimeCharts.Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeCharts.Domain.Events
{
    public class GenerateHeartDataEvent : Event
    {
        public GenerateHeartDataEvent(int max, int step)
        {
            Max = max;
            Step = step;
        }

        public int Max { get; set; }
        public int Step { get; set; }
    }
}
