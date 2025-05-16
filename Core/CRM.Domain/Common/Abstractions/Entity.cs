namespace CRM.Domain.Common.Abstractions;
public abstract class Entity<KeyType> : IEntity<KeyType>
{
    public KeyType Id { get; set; } = default!;
}
