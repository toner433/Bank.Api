using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.Models;

namespace Bank.Domain.Models
{
    public class Card
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Account Account { get; set; }
        public Guid AccountId { get; set; }
        public string CardNumber { get; set; }
        public string ValidDate    { get; set; }
        public string CardholderName    { get; set; }
        public string CardType { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }



    }
}
