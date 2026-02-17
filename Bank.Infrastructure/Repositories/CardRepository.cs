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
    public class CardRepository:ICardRepository
    {
        private readonly BankDbContext _context;
        public CardRepository(BankDbContext context)
        {
            _context = context;
        }
        public async Task<Card?> GetByIdAsync(Guid id)
        {
            return await _context.Cards
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<List<Card>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Cards
                .Where(a => a.AccountId == accountId)
                .ToListAsync();
        }
        public async Task<List<Card>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Cards
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }
        public async Task AddAsync(Card card)
        {
            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();

        }
        public async Task UpdateAsync(Card card)
        {
            _context.Cards.Update(card);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteByIdAsync(Guid id)
        {
            await _context.Cards
                .Where(a => a.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
