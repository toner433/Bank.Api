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
    public class CardRepository : ICardRepository
    {
        private readonly IDataBaseRepository _repository;

        public CardRepository(IDataBaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Card>> GetByAccountIdAsync(Guid accountId)
        {
            var cards = await _repository.GetAllAsync<Card>();
            return cards.Where(c => c.AccountId == accountId).ToList();
        }

        public async Task<List<Card>> GetByUserIdAsync(Guid userId)
        {
            var cards = await _repository.GetAllAsync<Card>();
            return cards.Where(c => c.UserId == userId).ToList();
        }

        public async Task<Card?> GetByCardNumberAsync(string cardNumber)
        {
            var cards = await _repository.GetAllAsync<Card>();
            return cards.FirstOrDefault(c => c.CardNumber == cardNumber);
        }

        public async Task<CardOperation?> GetOperationByIdAsync(Guid id)
        {
            var operations = await _repository.GetAllAsync<CardOperation>();
            return operations.FirstOrDefault(o => o.Id == id);
        }

        public async Task<List<CardOperation>> GetOperationsByCardIdAsync(Guid cardId)
        {
            var operations = await _repository.GetAllAsync<CardOperation>();
            return operations.Where(o => o.CardId == cardId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        public async Task AddOperationAsync(CardOperation operation)
        {
            await _repository.AddAsync(operation);
        }
    }
}
