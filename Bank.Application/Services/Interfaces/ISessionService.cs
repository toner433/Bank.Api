using Bank.Application.DTOs.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Services.Interfaces
{
    public interface ISessionService
    {
        Task<SessionDto> CreateSessionAsync(Guid userId);
        Task<SessionDto?> GetByTokenAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
        Task LogoutAsync(string token);
        Task LogoutAllAsync(Guid userId);
        Task<List<SessionDto>> GetUserSessionsAsync(Guid userId);
        Task<int> CleanExpiredSessionsAsync();
    }
}
