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
    public class AuthRepository : IAuthRepository
    {
        private readonly IDataBaseRepository _repository;

        public AuthRepository(IDataBaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<Session?> GetSessionByTokenAsync(string token)
        {
            var sessions = await _repository.GetAllAsync<Session>();
            return sessions.FirstOrDefault(s => s.SessionToken == token);
        }

        public async Task DeleteExpiredSessionsAsync()
        {
            var sessions = await _repository.GetAllAsync<Session>();
            var expired = sessions.Where(s => s.ExpiresAt < DateTime.UtcNow).ToList();

            foreach (var session in expired)
            {
                await _repository.DeleteAsync<Session>(session.Id);
            }
        }

        public async Task<VerificationCode?> GetValidCodeAsync(Guid userId, string purpose, string code)
        {
            var codes = await _repository.GetAllAsync<VerificationCode>();
            return codes.Where(v => v.UserId == userId &&
                v.Purpose == purpose &&
                v.Code == code &&
                !v.IsUsed &&
                v.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefault();
        }

        public async Task MarkCodeAsUsedAsync(Guid id)
        {
            var code = await _repository.GetByIdAsync<VerificationCode>(id);
            if (code != null)
            {
                code.IsUsed = true;
                await _repository.UpdateAsync(code);
            }
        }

        public async Task DeleteExpiredCodesAsync()
        {
            var codes = await _repository.GetAllAsync<VerificationCode>();
            var expired = codes.Where(v => v.ExpiresAt < DateTime.UtcNow || v.IsUsed).ToList();

            foreach (var code in expired)
            {
                await _repository.DeleteAsync<VerificationCode>(code.Id);
            }
        }
    }

}
