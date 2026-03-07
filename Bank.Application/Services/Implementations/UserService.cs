using System;
using System.Threading.Tasks;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;
using Bank.Application.DTOs.Users;
using Bank.Application.DTOs.Common;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;



namespace Bank.Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IDataBaseRepository _dbRepository;

        public UserService(
            IUserRepository userRepository,
            IAuthRepository authRepository,
            IDataBaseRepository dbRepository)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _dbRepository = dbRepository;
        }
        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Login = user.Login,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                IsBlocked = user.IsBlocked,
                CreatedAt = user.CreatedAt
            };
        }

        private string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-");
        }
        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _dbRepository.GetByIdAsync<User>(id);
            if (user == null) return null;
            return MapToUserDto(user);
        }

        public async Task<UserDto?> GetByLoginAsync(string login)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null) return null;
            return MapToUserDto(user);
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var exists = await _userRepository.ExistsAsync(request.Login);
            if (exists) throw new BusinessException("Логин уже занят");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = request.Login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Email = request.Email,
                Phone = request.Phone,
                FullName = request.FullName,
                PassportNumber = request.PassportNumber,  
                BirthDate = request.BirthDate,
                IsBlocked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _dbRepository.AddAsync(user);

            var session = new Session
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                SessionToken = GenerateToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsActive = true
            };

            await _dbRepository.AddAsync(session);

            return new AuthResponse
            {
                Token = session.SessionToken,
                ExpiresAt = session.ExpiresAt,
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByLoginAsync(request.Login);
            if (user == null) throw new UnauthorizedException("Неверный логин или пароль");
            if (user.IsBlocked) throw new UnauthorizedException("Пользователь заблокирован");

            bool passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordIsValid) throw new UnauthorizedException("Неверный логин или пароль");

            var session = new Session
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                SessionToken = GenerateToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                IsActive = true
            };

            await _dbRepository.AddAsync(session);

            return new AuthResponse
            {
                Token = session.SessionToken,
                ExpiresAt = session.ExpiresAt,
                User = MapToUserDto(user)
            };
        }

        public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _dbRepository.GetByIdAsync<User>(userId);
            if (user == null) throw new NotFoundException("Пользователь не найден");

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrEmpty(request.Email))
                user.Email = request.Email;

            if (!string.IsNullOrEmpty(request.Phone))
                user.Phone = request.Phone;

            await _dbRepository.UpdateAsync(user);
            return MapToUserDto(user);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var user = await _dbRepository.GetByIdAsync<User>(userId);
            if (user == null) throw new NotFoundException("Пользователь не найден");

            bool oldPasswordIsValid = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash);
            if (!oldPasswordIsValid) throw new BusinessException("Неверный старый пароль");

            if (request.NewPassword != request.ConfirmPassword)
                throw new BusinessException("Новый пароль и подтверждение не совпадают");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _dbRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ExistsAsync(string login)
        {
            return await _userRepository.ExistsAsync(login);
        }
       
    }
}