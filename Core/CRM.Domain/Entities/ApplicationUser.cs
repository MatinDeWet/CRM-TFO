using Microsoft.AspNetCore.Identity;

namespace CRM.Domain.Entities;
public class ApplicationUser : IdentityUser<int>
{
    public virtual User User { get; set; } = null!;
}
