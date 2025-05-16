using CRM.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM.Persistence.Data.Context;
public class CrmContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public CrmContext() { }

    public CrmContext(DbContextOptions<CrmContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(CrmContext).Assembly);

        base.OnModelCreating(builder);
    }
}
