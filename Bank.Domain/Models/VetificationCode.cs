using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.Models;

namespace Bank.Domain.Models
{
    public class VetificationCode
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string Code { get; set; }
        public string Purpose { get; set; }
        public bool IsUsed { get; set; }
        public DateTime ValidAt { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
