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
        Task AddSessionAsync(Session session);
        Task DeleteSessionAsync(Guid id);
        Task DeleteExpiredSessionsAsync();
        Task<VerificationCode?> GetValidCodeAsync(Guid userId, string purpose, string code);
        Task AddVerificationCodeAsync(VerificationCode verificationCode);
        Task MarkCodeAsUsedAsync(Guid id);
        Task DeleteExpiredCodesAsync();
    }
}
