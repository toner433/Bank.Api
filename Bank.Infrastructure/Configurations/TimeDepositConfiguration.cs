using Bank.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bank.Infrastructure.Configurations
{
    public class TimeDepositConfiguration : IEntityTypeConfiguration<TimeDeposit>
    {
        public void Configure(EntityTypeBuilder<TimeDeposit> builder)
        {
            builder.ToTable("time_deposits");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            builder.Property(x => x.Principal).HasColumnName("principal").HasPrecision(15, 2);
            builder.Property(x => x.AnnualRatePercent).HasColumnName("annual_rate_percent").HasPrecision(6, 2);
            builder.Property(x => x.TermMonths).HasColumnName("term_months");
            builder.Property(x => x.OpenedAt).HasColumnName("opened_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.MaturityDate).HasColumnName("maturity_date");
            builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Active");
            builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.Organization).WithMany(x => x.TimeDeposits).HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(x => x.DepositAccount).WithMany().HasForeignKey(x => x.DepositAccountId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
