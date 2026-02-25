using Bank.Application.DTOs.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Services.Interfaces
{
    public interface IVerificationCodeService
    {
        Task<CodeResponse> GenerateCodeAsync(GenerateCodeRequest request);
        Task<bool> VerifyCodeAsync(VerifyCodeRequest request);
        Task<bool> SendCodeBySmsAsync(string phoneNumber, string code);
        Task<bool> SendCodeByEmailAsync(string email, string code);
        Task<int> CleanExpiredCodesAsync();
    }
}
