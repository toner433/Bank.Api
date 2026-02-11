using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bank.Domain.Models;

namespace Bank.DataAccess.Configurations
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.ToTable("cards");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(x => x.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            builder.Property(x => x.CardNumber)
                .HasColumnName("card_number_masked")
                .HasMaxLength(19)
                .IsRequired();

            builder.Property(x => x.ExpiryDate)
                .HasColumnName("expiry_date")
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(x => x.CardholderName)
                .HasColumnName("cardholder_name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.CardType)
                .HasColumnName("card_type")
                .HasMaxLength(10)
                .HasDefaultValue("debit");

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasMaxLength(10)
                .HasDefaultValue("active");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(x => x.User)
                .WithMany(x => x.Cards)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Account)
                .WithMany(x => x.Cards)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}