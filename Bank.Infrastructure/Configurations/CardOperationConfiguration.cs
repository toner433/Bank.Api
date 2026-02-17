using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bank.Domain.Models;

namespace Bank.Infrastructure.Configurations
{
    public class CardOperationConfiguration : IEntityTypeConfiguration<CardOperation>
    {
        public void Configure(EntityTypeBuilder<CardOperation> builder)
        {
            builder.ToTable("card_operations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.CardId)
                .HasColumnName("card_id")
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(x => x.StoreName)
                .HasColumnName("store_name")
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasMaxLength(10)
                .HasDefaultValue("completed");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(x => x.Card)
                .WithMany(x => x.CardOperations)
                .HasForeignKey(x => x.CardId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}