using System;
using System.Threading.Tasks;
using Bank.Application.DTOs.Deposits;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepositsController : ControllerBase
    {
        private readonly ITimeDepositService _timeDepositService;

        public DepositsController(ITimeDepositService timeDepositService)
        {
            _timeDepositService = timeDepositService;
        }

        [HttpPost("open")]
        public async Task<IActionResult> Open([FromBody] OpenTimeDepositRequest request)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var dto = await _timeDepositService.OpenAsync(request, uid.Value);
                return Ok(dto);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyDeposits()
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            var list = await _timeDepositService.ListForUserAsync(uid.Value);
            return Ok(list);
        }

        [HttpGet("organization/{organizationId:guid}")]
        public async Task<IActionResult> OrganizationDeposits(Guid organizationId)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var list = await _timeDepositService.ListForOrganizationAsync(organizationId, uid.Value);
                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("close")]
        public async Task<IActionResult> Close([FromBody] CloseTimeDepositRequest request)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var dto = await _timeDepositService.CloseAsync(request, uid.Value);
                return Ok(dto);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
