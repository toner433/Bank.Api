using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface IVerificationCodeRepository
    {
        Task<VerificationCode?> GetValidCodeAsync(Guid userId, string purpose, string code);
        Task AddAsync(VerificationCode verificationCode);
        Task MarkAsUsedAsync(Guid id);
        Task DeleteExpiredAsync();
    }
}

