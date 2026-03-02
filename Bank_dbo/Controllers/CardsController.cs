using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;
using CreateCardRequest = Bank.Application.DTOs.Cards.CreateCardRequest;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CardsController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardsController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var card = await _cardService.GetByIdAsync(id);
            if (card == null)
                return NotFound();
            return Ok(card);
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetByAccountId(Guid accountId)
        {
            var cards = await _cardService.GetByAccountIdAsync(accountId);
            return Ok(cards);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var cards = await _cardService.GetByUserIdAsync(userId);
            return Ok(cards);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(CreateCardRequest request)
        {
            try
            {
                var result = await _cardService.CreateCardAsync(request);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/block")]
        public async Task<IActionResult> BlockCard(Guid id)
        {
            try
            {
                var result = await _cardService.BlockCardAsync(id);
                return Ok(new { success = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/unblock")]
        public async Task<IActionResult> UnblockCard(Guid id)
        {
            try
            {
                var result = await _cardService.UnblockCardAsync(id);
                return Ok(new { success = result });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}