using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.Models;

namespace Bank.Domain.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string SessionToken { get; set; }
        public DateTime ValidAt { get; set; }
        public DateTime CreatedAt   { get; set; }


    }
}
