using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.DTOs.Cards
{
    public class CreateCardRequest
    {
        public Guid AccountId { get; set; }
        public string CardType { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
    }
}
