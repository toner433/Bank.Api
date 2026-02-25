using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.DTOs.Verification
{
    public class VerifyCodeRequest
    {
        public Guid UserId { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
