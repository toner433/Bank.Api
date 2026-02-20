using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<Session?> GetSessionByTokenAsync(string token);
        Task DeleteExpiredSessionsAsync();
        Task<VerificationCode?> GetValidCodeAsync(Guid userId, string purpose, string code);
        Task MarkCodeAsUsedAsync(Guid id);
        Task DeleteExpiredCodesAsync();
    }
}
