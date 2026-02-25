using Bank.Application.DTOs.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Services.Interfaces
{
    public interface ICardService
    {
        Task<CardDto?> GetByIdAsync(Guid id);
        Task<List<CardDto>> GetByAccountIdAsync(Guid accountId);
        Task<List<CardDto>> GetByUserIdAsync(Guid userId);
        Task<CardDto> CreateCardAsync(CreateCardRequest request);
        Task<bool> BlockCardAsync(Guid cardId);
        Task<bool> UnblockCardAsync(Guid cardId);
    }
}
