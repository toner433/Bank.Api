using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bank.Domain.Models;

namespace Bank.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.Login)
                .HasColumnName("login")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasColumnName("phone")
                .HasMaxLength(20);

            builder.Property(x => x.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(255);

            builder.Property(x => x.PassportNumber)
                .HasColumnName("passport_number")
                .HasMaxLength(6);

            builder.Property(x => x.BirthDate)
                .HasColumnName("birth_date");

            builder.Property(x => x.IsBlocked)
                .HasColumnName("is_blocked")
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(x => x.Login)
                .IsUnique();
        }
    }
}