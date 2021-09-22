using MediatR;
using OperationResult;
using System;

namespace RealTimeCharts.Shared.Events
{
    public abstract class Event : IRequest<Result>
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
