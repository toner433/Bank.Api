using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bank.Domain.Models;
using Bank.Infrastructure.Configurations;

namespace Bank.Infrastructure.Context
{
    public class BankDbContext: DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<AccountOperation> AccountOperations => Set<AccountOperation>();
        public DbSet<CardOperation> CardOperations => Set<CardOperation>();
        public DbSet<OperationType> OperationTypes => Set<OperationType>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<VerificationCode> VerificationCodes => Set<VerificationCode>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new OperationTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AccountOperationConfiguration());
            modelBuilder.ApplyConfiguration(new SessionConfiguration());
            modelBuilder.ApplyConfiguration(new VerificationCodeConfiguration());
            modelBuilder.ApplyConfiguration(new CardConfiguration());
            modelBuilder.ApplyConfiguration(new CardOperationConfiguration());
        }
    }
}

