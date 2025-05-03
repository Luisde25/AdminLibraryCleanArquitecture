using AdminLibrary.Shared;
using AdminLibrary.SharedKernel.Interfaces;
using MediatR;

namespace AdminLibrary.SharedKernel
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;
        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task DispatcheAndClearEvents(IEnumerable<EntityBase<int>> entitiesWithEvents)
        {
            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.DomainEvents.ToArray();

                foreach (var domainEvent in events)
                {
                    await _mediator.Publish(domainEvent).ConfigureAwait(false); 
                }
            }
        }
    }
}
