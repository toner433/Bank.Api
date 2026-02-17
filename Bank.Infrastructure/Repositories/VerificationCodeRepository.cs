using Bank.Domain.Interfaces;
using Bank.Domain.Models;
using Bank.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Repositories
{
    public class VerificationCodeRepository : IVerificationCodeRepository
    {
        private readonly BankDbContext _context;

        public VerificationCodeRepository(BankDbContext context)
        {
            _context = context;
        }

        public async Task<VerificationCode?> GetValidCodeAsync(Guid userId, string purpose, string code)
        {
            return await _context.VerificationCodes
                .Where(v => v.UserId == userId &&
                            v.Purpose == purpose &&
                            v.Code == code &&
                            !v.IsUsed &&
                            v.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(VerificationCode verificationCode)
        {
            await _context.VerificationCodes.AddAsync(verificationCode);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsUsedAsync(Guid id)
        {
            var code = await _context.VerificationCodes.FindAsync(id);
            if (code != null)
            {
                code.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredAsync()
        {
            var expired = await _context.VerificationCodes
                .Where(v => v.ExpiresAt < DateTime.UtcNow || v.IsUsed)
                .ToListAsync();

            if (expired.Any())
            {
                _context.VerificationCodes.RemoveRange(expired);
                await _context.SaveChangesAsync();
            }
        }
    }
}
