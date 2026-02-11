using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bank.Domain.Models;

namespace Bank.DataAccess.Configurations
{
    public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
    {
        public void Configure(EntityTypeBuilder<VerificationCode> builder)
        {
            builder.ToTable("verification_codes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.UserId)
                .HasColumnName("user_id");

            builder.Property(x => x.Code)
                .HasColumnName("code")
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Purpose)
                .HasColumnName("purpose")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.IsUsed)
                .HasColumnName("is_used")
                .HasDefaultValue(false);

            builder.Property(x => x.ExpiresAt)
                .HasColumnName("expires_at")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(x => x.User)
                .WithMany(x => x.VerificationCodes)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}