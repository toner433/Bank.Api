using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _sessionService.LogoutAsync(token);
            return Ok(new { message = "Выход выполнен" });
        }

        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                await _sessionService.LogoutAllAsync(userId);
                return Ok(new { message = "Все сессии завершены" });
            }
            return BadRequest(new { error = "Не удалось определить пользователя" });
        }

        [HttpGet("my-sessions")]
        public async Task<IActionResult> GetMySessions()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                var sessions = await _sessionService.GetUserSessionsAsync(userId);
                return Ok(sessions);
            }
            return BadRequest(new { error = "Не удалось определить пользователя" });
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentSession()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var session = await _sessionService.GetByTokenAsync(token);
            if (session == null)
                return NotFound();
            return Ok(session);
        }
    }
}