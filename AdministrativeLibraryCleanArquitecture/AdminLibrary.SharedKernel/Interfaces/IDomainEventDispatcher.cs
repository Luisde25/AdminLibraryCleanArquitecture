using AdminLibrary.Shared;

namespace AdminLibrary.SharedKernel.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatcheAndClearEvents(IEnumerable<EntityBase<int>> entitiesWithEvents);
    }
}
