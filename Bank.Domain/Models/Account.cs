using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Domain.Models;

namespace Bank.Domain.Models
{
    public class Account
    { 
        public Guid Id {  get; set; }
        public User User {  get; set; }
        public Guid UserId { get; set; }
        public string AccountNumber { get; set; }
        public string  Balance     { get; set; }
        public string Currency {  get; set; }
        public string AccountType { get; set; }
        public DateTime OpenedDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
