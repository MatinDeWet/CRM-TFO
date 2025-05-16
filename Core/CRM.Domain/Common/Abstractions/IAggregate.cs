using CRM.Domain.Common.Messaging;

namespace CRM.Domain.Common.Abstractions;
public interface IAggregate<KeyType> : IAggregate, IEntity<KeyType>
{
}

public interface IAggregate
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IDomainEvent[] ClearDomainEvents();
}
