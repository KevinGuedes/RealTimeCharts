using System;

namespace RealTimeCharts.Shared.Events
{
    public abstract class Event
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; protected set; }

        protected Event()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }
    }
}
