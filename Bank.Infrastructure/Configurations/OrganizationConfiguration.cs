using Bank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Configurations
{
    public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("organizations");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(300).IsRequired();
            builder.Property(x => x.Inn).HasColumnName("inn").HasMaxLength(12).IsRequired();
            builder.HasIndex(x => x.Inn).IsUnique();
            builder.Property(x => x.Kpp).HasColumnName("kpp").HasMaxLength(9);
            builder.Property(x => x.LegalAddress).HasColumnName("legal_address").HasMaxLength(500).IsRequired();
            builder.Property(x => x.PublicKeyPem).HasColumnName("public_key_pem");
            builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
