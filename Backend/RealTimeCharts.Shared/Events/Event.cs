using MediatR;
using OperationResult;
using System;

namespace RealTimeCharts.Shared.Events
{
    public abstract class Event : IRequest<Result>
    {
        public Guid Id { get; private set; }
        public DateTime CreationDate { get; private set; }
        public bool ShouldBeNacked { get => RetryCount > 5; }
        public int RetryCount { get; set; }

        protected Event()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            RetryCount = 0;
        }
    }
}
