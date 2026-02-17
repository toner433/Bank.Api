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
        Task<Card?> GetByIdAsync(Guid id);
        Task<List<Card>> GetByAccountIdAsync(Guid accountId);
        Task<List<Card>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Card card);
        Task UpdateAsync(Card card);
        Task DeleteByIdAsync(Guid id);
    }
}
