using System;
using System.Linq;
using System.Threading.Tasks;
using Bank.Application.DTOs.Accounts;
using Bank.Application.DTOs.Operations;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IOperationService _operationService;

        public AccountsController(IAccountService accountService, IOperationService operationService)
        {
            _accountService = accountService;
            _operationService = operationService;
        }

        [HttpGet("accessible")]
        public async Task<IActionResult> GetAccessible()
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            var accounts = await _accountService.GetAccessibleAccountsAsync(uid.Value);
            return Ok(accounts);
        }

        [HttpGet("transfer-recipient")]
        public async Task<IActionResult> GetTransferRecipient([FromQuery] string accountNumber)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            var preview = await _accountService.LookupTransferRecipientAsync(accountNumber);
            if (preview == null) return NotFound(new { error = "Счёт не найден" });
            return Ok(preview);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var account = await _accountService.GetByIdAsync(id, uid.Value);
                if (account == null) return NotFound();
                return Ok(account);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var accounts = await _accountService.GetByUserIdAsync(userId, uid.Value);
                return Ok(accounts);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var result = await _accountService.CreateAccountAsync(request, uid.Value);
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

        [HttpGet("{id:guid}/balance")]
        public async Task<IActionResult> GetBalance(Guid id)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var balance = await _accountService.GetBalanceAsync(id, uid.Value);
                return Ok(new { accountId = id, balance });
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

        [HttpGet("{id:guid}/history")]
        public async Task<IActionResult> GetHistory(
            Guid id,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int? limit)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            var filter = new OperationFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                Limit = limit
            };
            try
            {
                var history = await _accountService.GetAccountHistoryAsync(id, filter, uid.Value);
                return Ok(history);
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

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] UserBankTransferRequest request)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                TransferRequest transferRequest;
                if (request.ToAccountId.HasValue)
                {
                    var accs = await _accountService.GetAccessibleAccountsAsync(uid.Value);
                    if (!accs.Any(a => a.Id == request.ToAccountId.Value))
                        return BadRequest(new { error = "Счёт получателя должен быть из ваших доступных счетов" });

                    transferRequest = new TransferRequest
                    {
                        FromAccountId = request.FromAccountId,
                        ToAccountId = request.ToAccountId.Value,
                        Amount = request.Amount,
                        Description = string.IsNullOrWhiteSpace(request.Description) ? "Перевод" : request.Description,
                        RecipientInn = request.RecipientInn
                    };
                }
                else if (!string.IsNullOrWhiteSpace(request.ToAccountNumber))
                {
                    transferRequest = new TransferRequest
                    {
                        FromAccountId = request.FromAccountId,
                        ToAccountNumber = request.ToAccountNumber,
                        Amount = request.Amount,
                        Description = string.IsNullOrWhiteSpace(request.Description) ? "Перевод" : request.Description,
                        RecipientInn = request.RecipientInn
                    };
                }
                else
                    return BadRequest(new { error = "Укажите номер счёта получателя или выберите счёт из списка" });

                var result = await _operationService.TransferAsync(transferRequest, uid.Value);
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

        [HttpPost("{id:guid}/deposit")]
        public async Task<IActionResult> Deposit(Guid id, [FromBody] decimal amount)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var result = await _accountService.DepositAsync(id, amount, uid.Value);
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

        [HttpPost("{id:guid}/withdraw")]
        public async Task<IActionResult> Withdraw(Guid id, [FromBody] decimal amount)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var result = await _accountService.WithdrawAsync(id, amount, uid.Value);
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
