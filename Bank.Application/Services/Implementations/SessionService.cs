using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;
using Bank.Application.DTOs.Sessions;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;

namespace Bank.Application.Services.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IDataBaseRepository _dbRepository;

        public SessionService(IAuthRepository authRepository, IDataBaseRepository dbRepository)
        {
            _authRepository = authRepository;
            _dbRepository = dbRepository;
        }

        private SessionDto MapToSessionDto(Session session)
        {
            return new SessionDto
            {
                Id = session.Id,
                Token = session.SessionToken,
                CreatedAt = session.CreatedAt,
                ExpiresAt = session.ExpiresAt,
                IsActive = session.IsActive,
                UserId = session.UserId,
                UserLogin = session.User.Login 
            };
        }

        private string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-");
        }

        public async Task<SessionDto> CreateSessionAsync(Guid userId)
        {
            var user = await _dbRepository.GetByIdAsync<User>(userId);
            if (user == null) throw new NotFoundException("Пользователь не найден");

            var session = new Session
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SessionToken = GenerateToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsActive = true
            };

            await _dbRepository.AddAsync(session);
            return MapToSessionDto(session);
        }

        public async Task<SessionDto?> GetByTokenAsync(string token)
        {
            var session = await _authRepository.GetSessionByTokenAsync(token);
            if (session == null) return null;

            return MapToSessionDto(session);
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var session = await _authRepository.GetSessionByTokenAsync(token);

            if (session == null) return false;
            if (!session.IsActive) return false;
            if (session.ExpiresAt < DateTime.UtcNow) return false;

            return true;
        }

        public async Task LogoutAsync(string token)
        {
            var session = await _authRepository.GetSessionByTokenAsync(token);
            if (session != null)
            {
                session.IsActive = false;
                await _dbRepository.UpdateAsync(session);
            }
        }

        public async Task LogoutAllAsync(Guid userId)
        {
            var sessions = await _dbRepository.GetAllAsync<Session>();

            foreach (var session in sessions.Where(s => s.UserId == userId && s.IsActive))
            {
                session.IsActive = false;
                await _dbRepository.UpdateAsync(session);
            }
        }

        public async Task<List<SessionDto>> GetUserSessionsAsync(Guid userId)
        {
            var sessions = await _dbRepository.GetAllAsync<Session>();

            return sessions
                .Where(s => s.UserId == userId)
                .Select(MapToSessionDto)
                .ToList();
        }

        public async Task<int> CleanExpiredSessionsAsync()
        {
            await _authRepository.DeleteExpiredSessionsAsync();
            return 0;
        }
    }
}