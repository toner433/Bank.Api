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
    public class AuthRepository : IAuthRepository
    {
        private readonly BankDbContext _context;

        public AuthRepository(BankDbContext context)
        {
            _context = context;
        }

        public async Task<Session?> GetSessionByTokenAsync(string token)
        {
            return await _context.Sessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SessionToken == token);
        }

        public async Task AddSessionAsync(Session session)
        {
            await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSessionAsync(Guid id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session != null)
            {
                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredSessionsAsync()
        {
            var expired = await _context.Sessions
                .Where(s => s.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            if (expired.Any())
            {
                _context.Sessions.RemoveRange(expired);
                await _context.SaveChangesAsync();
            }
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

        public async Task AddVerificationCodeAsync(VerificationCode verificationCode)
        {
            await _context.VerificationCodes.AddAsync(verificationCode);
            await _context.SaveChangesAsync();
        }

        public async Task MarkCodeAsUsedAsync(Guid id)
        {
            var code = await _context.VerificationCodes.FindAsync(id);
            if (code != null)
            {
                code.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredCodesAsync()
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
