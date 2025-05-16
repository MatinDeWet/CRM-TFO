namespace CRM.Domain.Common.Abstractions;
public interface IEntity<KeyType>
{
    KeyType Id { get; set; }
}
