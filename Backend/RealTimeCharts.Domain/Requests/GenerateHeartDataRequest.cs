using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeCharts.Domain.Requests
{
    public class GenerateHeartDataRequest
    {
        public GenerateHeartDataRequest(int max, int step)
        {
            Max = max;
            Step = step;
        }

        public int Max { get; set; }
        public int Step { get; set; }
    }
}
