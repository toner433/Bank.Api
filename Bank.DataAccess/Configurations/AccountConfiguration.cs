using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bank.Domain.Models;

namespace Bank.Infrastructure.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.UserId)
                .HasColumnName("user_id");

            builder.Property(x => x.AccountNumber)
                .HasColumnName("account_number")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Balance)
                .HasColumnName("balance")
                .HasPrecision(15, 2)
                .HasDefaultValue(0.00m);

            builder.Property(x => x.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .HasDefaultValue("BYN");

            builder.Property(x => x.AccountType)
                .HasColumnName("account_type")
                .HasMaxLength(30)
                .HasDefaultValue("current");

            builder.Property(x => x.OpenedAt)
                .HasColumnName("opened_date")
                .HasDefaultValueSql("CURRENT_DATE");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(x => x.AccountNumber)
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Accounts)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}