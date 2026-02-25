using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.DTOs.Sessions
{
    public class SessionDto
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
        public string UserLogin { get; set; } = string.Empty;
    }
}
