using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;
using Bank.Application.DTOs.Cards;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;

namespace Bank.Application.Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IDataBaseRepository _dbRepository;

        public CardService(
            ICardRepository cardRepository,
            IAccountRepository accountRepository,
            IDataBaseRepository dbRepository)
        {
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
            _dbRepository = dbRepository;
        }

        private async Task<CardDto> MapToCardDto(Card card)
        {
            var account = await _dbRepository.GetByIdAsync<Account>(card.AccountId);

            return new CardDto
            {
                Id = card.Id,
                CardNumber = MaskCardNumber(card.CardNumber),
                CardHolderName = card.CardHolderName,
                ExpiryDate = card.ExpiryDate,
                CardType = card.CardType,
                IsBlocked = card.IsBlocked,
                AccountId = card.AccountId,
                AccountNumber = account.AccountNumber 
            };
        }

        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return "****";

            return "**** **** **** " + cardNumber.Substring(cardNumber.Length - 4);
        }

        private string GenerateCardNumber()
        {
            var random = new Random();
            var number = "";
            for (int i = 0; i < 16; i++)
            {
                number += random.Next(0, 10).ToString();
            }
            return number;
        }

        public async Task<CardDto?> GetByIdAsync(Guid id)
        {
            var card = await _dbRepository.GetByIdAsync<Card>(id);
            if (card == null) return null;

            return await MapToCardDto(card);
        }

        public async Task<List<CardDto>> GetByAccountIdAsync(Guid accountId)
        {
            var cards = await _cardRepository.GetByAccountIdAsync(accountId);
            var result = new List<CardDto>();

            foreach (var card in cards)
            {
                result.Add(await MapToCardDto(card));
            }

            return result;
        }

        public async Task<List<CardDto>> GetByUserIdAsync(Guid userId)
        {
            var cards = await _cardRepository.GetByUserIdAsync(userId);
            var result = new List<CardDto>();

            foreach (var card in cards)
            {
                result.Add(await MapToCardDto(card));
            }

            return result;
        }

        public async Task<CardDto> CreateCardAsync(CreateCardRequest request)
        {
            var account = await _dbRepository.GetByIdAsync<Account>(request.AccountId);
            if (account == null) throw new NotFoundException("Счет не найден");

            var card = new Card
            {
                Id = Guid.NewGuid(),
                AccountId = request.AccountId,
                CardNumber = GenerateCardNumber(),
                CardHolderName = request.CardHolderName,
                ExpiryDate = DateTime.UtcNow.AddYears(3).ToString("MM/yy"),
                CardType = request.CardType,
                IsBlocked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _dbRepository.AddAsync(card);
            return await MapToCardDto(card);
        }

        public async Task<bool> BlockCardAsync(Guid cardId)
        {
            var card = await _dbRepository.GetByIdAsync<Card>(cardId);
            if (card == null) throw new NotFoundException("Карта не найдена");

            card.IsBlocked = true;
            await _dbRepository.UpdateAsync(card);
            return true;
        }

        public async Task<bool> UnblockCardAsync(Guid cardId)
        {
            var card = await _dbRepository.GetByIdAsync<Card>(cardId);
            if (card == null) throw new NotFoundException("Карта не найдена");

            card.IsBlocked = false;
            await _dbRepository.UpdateAsync(card);
            return true;
        }
    }
}