using Bank.Application.DTOs.Common;
using Bank.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<UserDto?> GetByLoginAsync(string login);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<bool> ExistsAsync(string login);
    }
}
