using System;

namespace Bank.Domain.Models
{
    public class CardOperation
    {
        public Guid Id { get; set; }
        public Guid CardId { get; set; }
        public Card Card { get; set; }
        public decimal Amount { get; set; }
        public string StoreName { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}