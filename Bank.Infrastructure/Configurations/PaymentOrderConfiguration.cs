using Bank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Configurations
{
    public class PaymentOrderConfiguration : IEntityTypeConfiguration<PaymentOrder>
    {
        public void Configure(EntityTypeBuilder<PaymentOrder> builder)
        {
            builder.ToTable("payment_orders");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            builder.Property(x => x.Amount).HasColumnName("amount").HasPrecision(15, 2);
            builder.Property(x => x.RecipientName).HasColumnName("recipient_name").HasMaxLength(300).IsRequired();
            builder.Property(x => x.RecipientInn).HasColumnName("recipient_inn").HasMaxLength(12);
            builder.Property(x => x.RecipientAccountNumber).HasColumnName("recipient_account_number").HasMaxLength(20);
            builder.Property(x => x.Purpose).HasColumnName("purpose").HasMaxLength(500).IsRequired();
            builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Draft");
            builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.ExecutedAt).HasColumnName("executed_at");
            builder.HasOne(x => x.Organization).WithMany(x => x.PaymentOrders).HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.FromAccount).WithMany().HasForeignKey(x => x.FromAccountId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
