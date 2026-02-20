using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface ICardRepository
    {
        Task<List<Card>> GetByAccountIdAsync(Guid accountId);
        Task<List<Card>> GetByUserIdAsync(Guid userId);
        Task<Card?> GetByCardNumberAsync(string cardNumber);
        Task<CardOperation?> GetOperationByIdAsync(Guid id);
        Task<List<CardOperation>> GetOperationsByCardIdAsync(Guid cardId);
        Task AddOperationAsync(CardOperation operation);
    }
}
