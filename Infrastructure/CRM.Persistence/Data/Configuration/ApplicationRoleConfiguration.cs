using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Data.Configuration;
public partial class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> entity)
    {
        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ApplicationRole> entity);
}
