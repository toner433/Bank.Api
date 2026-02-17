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
    public class SessionRepository : ISessionRepository
    {
        private readonly BankDbContext _context;

        public SessionRepository(BankDbContext context)
        {
            _context = context;
        }

        public async Task<Session?> GetByTokenAsync(string token)
        {
            return await _context.Sessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SessionToken == token);
        }

        public async Task AddAsync(Session session)
        {
            await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session != null)
            {
                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredAsync()
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
    }
}
