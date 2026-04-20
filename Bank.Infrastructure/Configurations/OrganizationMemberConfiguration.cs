using Bank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Configurations
{
    public class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
    {
        public void Configure(EntityTypeBuilder<OrganizationMember> builder)
        {
            builder.ToTable("organization_members");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            builder.Property(x => x.Role).HasColumnName("role").HasMaxLength(30).IsRequired();
            builder.Property(x => x.JoinedAt).HasColumnName("joined_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.HasIndex(x => new { x.OrganizationId, x.UserId }).IsUnique();
            builder.HasOne(x => x.Organization).WithMany(x => x.Members).HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.User).WithMany(x => x.OrganizationMembers).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
