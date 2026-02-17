using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface ISessionRepository
    {
        Task<Session?> GetByTokenAsync(string token);
        Task AddAsync(Session session);
        Task DeleteAsync(Guid id);
        Task DeleteExpiredAsync();
    }
}
