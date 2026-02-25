using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.DTOs.Cards
{
    public class CardDto
    {
        public Guid Id { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public bool IsBlocked { get; set; }
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
    }
}
