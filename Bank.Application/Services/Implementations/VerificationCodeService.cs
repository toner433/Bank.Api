using System;
using System.Threading.Tasks;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;
using Bank.Application.DTOs.Verification;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Bank.Application.Services.Implementations
{
    public class VerificationCodeService : IVerificationCodeService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IDataBaseRepository _dbRepository;
        private readonly IConfiguration _configuration;

        public VerificationCodeService(
            IAuthRepository authRepository,
            IDataBaseRepository dbRepository,
            IConfiguration configuration)
        {
            _authRepository = authRepository;
            _dbRepository = dbRepository;
            _configuration = configuration;
        }

        private string GenerateRandomCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<CodeResponse> GenerateCodeAsync(GenerateCodeRequest request)
        {
            var user = await _dbRepository.GetByIdAsync<User>(request.UserId);
            if (user == null) throw new NotFoundException("Пользователь не найден");

            var code = new VerificationCode
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Code = GenerateRandomCode(),
                Purpose = request.Purpose,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            await _dbRepository.AddAsync(code);

            return new CodeResponse
            {
                Success = true,
                Message = "Код сгенерирован",
                ExpiresAt = code.ExpiresAt,
                AttemptsLeft = 3
            };
        }

        public async Task<bool> VerifyCodeAsync(VerifyCodeRequest request)
        {
            var validCode = await _authRepository.GetValidCodeAsync(
                request.UserId,
                request.Purpose,
                request.Code);

            if (validCode == null) return false;

            await _authRepository.MarkCodeAsUsedAsync(validCode.Id);
            return true;
        }

        public async Task<bool> SendCodeBySmsAsync(string phoneNumber, string code)
        {
            try
            {
                Console.WriteLine($"SMS отправлено на {phoneNumber}: Ваш код {code}");
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendCodeByEmailAsync(string email, string code)
        {
            try
            {
                var host = _configuration["Smtp:Host"];
                var portRaw = _configuration["Smtp:Port"];
                var username = _configuration["Smtp:Username"];
                var password = _configuration["Smtp:Password"];
                var from = _configuration["Smtp:From"] ?? username;
                var sslRaw = _configuration["Smtp:UseSsl"];

                if (!string.IsNullOrWhiteSpace(host)
                    && !string.IsNullOrWhiteSpace(portRaw)
                    && int.TryParse(portRaw, out var port)
                    && !string.IsNullOrWhiteSpace(from))
                {
                    using var client = new SmtpClient(host, port)
                    {
                        EnableSsl = !string.Equals(sslRaw, "false", StringComparison.OrdinalIgnoreCase),
                    };

                    if (!string.IsNullOrWhiteSpace(username))
                        client.Credentials = new NetworkCredential(username, password);

                    using var msg = new MailMessage(from, email)
                    {
                        Subject = "Сброс пароля D-bank",
                        Body = $"Ваш код для сброса пароля: {code}. Код действует 10 минут."
                    };
                    await client.SendMailAsync(msg);
                }
                else
                {
                    Console.WriteLine($"Email отправлено на {email}: Ваш код {code}");
                }
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> CleanExpiredCodesAsync()
        {
            return await _authRepository.DeleteExpiredCodesAsync();
        }
    }
}