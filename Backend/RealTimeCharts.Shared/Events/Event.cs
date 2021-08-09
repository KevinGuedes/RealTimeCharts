using System;

namespace PaymentContext.Shared.Events
{
    public abstract class Event
    {
        public DateTime CreationDate { get; protected set; }

        protected Event()
        {
            CreationDate = DateTime.Now;
        }
    }
}
