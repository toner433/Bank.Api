using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bank.Domain.Models;

namespace Bank.DataAccess.Configurations
{
    public class AccountOperationConfiguration : IEntityTypeConfiguration<AccountOperation>
    {
        public void Configure(EntityTypeBuilder<AccountOperation> builder)
        {
            builder.ToTable("operations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(x => x.FromAccountId)
                .HasColumnName("from_account_id");

            builder.Property(x => x.ToAccountId)
                .HasColumnName("to_account_id");

            builder.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasPrecision(15, 2)
                .IsRequired();

            builder.Property(x => x.OperationTypeId)
                .HasColumnName("operation_type_id");

            builder.Property(x => x.Description)
                .HasColumnName("description");

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .HasDefaultValue("pending");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.CompletedAt)
                .HasColumnName("completed_at");

            builder.HasOne(x => x.FromAccount)
                .WithMany(x => x.FromOperations)
                .HasForeignKey(x => x.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ToAccount)
                .WithMany(x => x.ToOperations)
                .HasForeignKey(x => x.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OperationType)
                .WithMany(x => x.AccountOperations)
                .HasForeignKey(x => x.OperationTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}