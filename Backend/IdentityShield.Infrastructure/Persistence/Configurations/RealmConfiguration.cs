using IdentityShield.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityShield.Infrastructure.Persistence.Configurations
{
    public class RealmConfiguration : IEntityTypeConfiguration<Realm>
    {
        public void Configure(EntityTypeBuilder<Realm> builder)
        {
            builder.Property(a => a.Name).IsRequired();
            builder.HasIndex(a => a.Name).IsUnique();
            builder.Property(a => a.Name).HasColumnType("NVARCHAR(256)");
            builder.Property(a => a.Id).HasDefaultValueSql("NEWID()");
        }
    }
}
