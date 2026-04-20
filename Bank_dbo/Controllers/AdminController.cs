using System;
using System.Threading.Tasks;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private Guid? CurrentUserId => User.GetCurrentUserId();

        [HttpGet("stats")]
        public async Task<IActionResult> Stats()
        {
            var uid = CurrentUserId;
            if (uid == null) return Unauthorized();
            try
            {
                var dto = await _adminService.GetStatsAsync(uid.Value);
                return Ok(dto);
            }
            catch (BusinessException ex) when (ex.Message.Contains("Доступ только"))
            {
                return Forbid();
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> Users()
        {
            var uid = CurrentUserId;
            if (uid == null) return Unauthorized();
            try
            {
                var list = await _adminService.ListUsersAsync(uid.Value);
                return Ok(list);
            }
            catch (BusinessException ex) when (ex.Message.Contains("Доступ только"))
            {
                return Forbid();
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("organizations")]
        public async Task<IActionResult> Organizations()
        {
            var uid = CurrentUserId;
            if (uid == null) return Unauthorized();
            try
            {
                var list = await _adminService.ListOrganizationsAsync(uid.Value);
                return Ok(list);
            }
            catch (BusinessException ex) when (ex.Message.Contains("Доступ только"))
            {
                return Forbid();
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("users/{id:guid}/block")]
        public async Task<IActionResult> BlockUser(Guid id)
        {
            var uid = CurrentUserId;
            if (uid == null) return Unauthorized();
            try
            {
                await _adminService.SetUserBlockedAsync(uid.Value, id, true);
                return Ok(new { success = true });
            }
            catch (BusinessException ex) when (ex.Message.Contains("Доступ только"))
            {
                return Forbid();
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

        [HttpPost("users/{id:guid}/unblock")]
        public async Task<IActionResult> UnblockUser(Guid id)
        {
            var uid = CurrentUserId;
            if (uid == null) return Unauthorized();
            try
            {
                await _adminService.SetUserBlockedAsync(uid.Value, id, false);
                return Ok(new { success = true });
            }
            catch (BusinessException ex) when (ex.Message.Contains("Доступ только"))
            {
                return Forbid();
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
