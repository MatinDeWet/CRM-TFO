using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Persistence.Data.Configuration;
public partial class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> entity)
    {
        entity.HasOne(x => x.User)
            .WithOne(x => x.IdentityInfo)
            .HasForeignKey<User>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);

        OnConfigurePartial(entity);
    }

    partial void OnConfigurePartial(EntityTypeBuilder<ApplicationUser> entity);
}
