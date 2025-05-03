using MediatR;

namespace AdminLibrary.SharedKernel
{
    public class DomainEventBase : INotification
    {
        public DateTime DateOcurred { get; protected set; } = DateTime.UtcNow.AddHours(-5);
    }
}
