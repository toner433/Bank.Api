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
    public class CardRepository : BaseRepository<Card>, ICardRepository
    {
        public CardRepository(BankDbContext context) : base(context)
        {
        }

        public async Task<List<Card>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Cards
                .Where(c => c.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<List<Card>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Cards
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Card?> GetByCardNumberAsync(string cardNumber)
        {
            return await _context.Cards
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);
        }


        public async Task<CardOperation?> GetOperationByIdAsync(Guid id)
        {
            return await _context.CardOperations
                .Include(o => o.Card)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<CardOperation>> GetOperationsByCardIdAsync(Guid cardId)
        {
            return await _context.CardOperations
                .Where(o => o.CardId == cardId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task AddOperationAsync(CardOperation operation)
        {
            await _context.CardOperations.AddAsync(operation);
            await _context.SaveChangesAsync();
        }
    }
}
