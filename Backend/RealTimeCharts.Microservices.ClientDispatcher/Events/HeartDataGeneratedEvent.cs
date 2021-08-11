﻿using RealTimeCharts.Shared.Events;
using RealTimeCharts.Domain.Models;

namespace RealTimeCharts.Microservices.ClientDispatcher.Events
{
    public class HeartDataGeneratedEvent : Event
    {
        public HeartDataGeneratedEvent(DataPoint dataPoint)
        {
            DataPoint = dataPoint;
        }

        public DataPoint DataPoint;
    }
}
