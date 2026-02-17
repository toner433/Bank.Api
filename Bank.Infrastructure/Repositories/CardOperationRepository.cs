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
    public class CardOperationRepository : ICardOperationRepository
    {
        private readonly BankDbContext _context;

        public CardOperationRepository(BankDbContext context)
        {
            _context = context;
        }

        public async Task<CardOperation?> GetByIdAsync(Guid id)
        {
            return await _context.CardOperations
                .Include(o => o.Card)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<CardOperation>> GetByCardIdAsync(Guid cardId)
        {
            return await _context.CardOperations
                .Where(o => o.CardId == cardId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(CardOperation operation)
        {
            await _context.CardOperations.AddAsync(operation);
            await _context.SaveChangesAsync();
        }
    }
}
