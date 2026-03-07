using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bank.Application.DTOs.Accounts;
using Bank.Application.DTOs.Operations;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IOperationService _operationService;

        public AccountsController(IAccountService accountService, IOperationService operationService)
        {
            _accountService = accountService;
            _operationService = operationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null) return NotFound();
            return Ok(account);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var accounts = await _accountService.GetByUserIdAsync(userId);
            return Ok(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountRequest request)
        {
            try
            {
                var result = await _accountService.CreateAccountAsync(request);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/balance")]
        public async Task<IActionResult> GetBalance(Guid id)
        {
            try
            {
                var balance = await _accountService.GetBalanceAsync(id);
                return Ok(new { accountId = id, balance });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetHistory(
            Guid id,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int? limit)
        {
            var filter = new OperationFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                Limit = limit
            };

            var history = await _accountService.GetAccountHistoryAsync(id, filter);
            return Ok(history);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(TransferRequest request)
        {
            try
            {
                var result = await _operationService.TransferAsync(request);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(Guid id, [FromBody] decimal amount)
        {
            try
            {
                var result = await _accountService.DepositAsync(id, amount);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(Guid id, [FromBody] decimal amount)
        {
            try
            {
                var result = await _accountService.WithdrawAsync(id, amount);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}