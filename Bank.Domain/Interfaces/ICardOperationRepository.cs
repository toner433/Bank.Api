using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface ICardOperationRepository
    {
        Task<CardOperation?> GetByIdAsync(Guid id);
        Task<List<CardOperation>> GetByCardIdAsync(Guid cardId);
        Task AddAsync(CardOperation operation);
    }
}
