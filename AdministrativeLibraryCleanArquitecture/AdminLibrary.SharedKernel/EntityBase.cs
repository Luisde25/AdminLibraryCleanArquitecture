using AdminLibrary.SharedKernel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminLibrary.Shared
{
    public abstract class EntityBase<Tid> where Tid : unmanaged
    {
        public Tid Id { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now.AddHours(-5);
        public string CreateUser { get; set; } = "Admin";
        public DateTime? UpdateDate { get; set; }
        public string? UpdateUser { get; set; }


        private readonly List<DomainEventBase> _domainEvents = new();
        [NotMapped]
        public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly(); 
        protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
        internal void ClearDomainEvents() => _domainEvents.Clear();
    }
}
