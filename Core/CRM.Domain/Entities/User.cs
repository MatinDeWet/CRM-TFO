using CRM.Domain.Common.Abstractions;

namespace CRM.Domain.Entities;
public class User : Entity<int>
{
    public virtual ApplicationUser IdentityInfo { get; set; } = null!;
}
