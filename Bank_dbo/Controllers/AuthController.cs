using Bank.Application.DTOs.Users;
using Bank.Application.Exceptions;
using Bank.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var response = await _userService.RegisterAsync(request);
                return Ok(response);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var response = await _userService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("password-reset/request")]
        public async Task<IActionResult> RequestPasswordReset(RequestPasswordResetRequest request)
        {
            try
            {
                await _userService.RequestPasswordResetAsync(request);
                return Ok(new { message = "Если email существует, код отправлен на почту" });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("password-reset/confirm")]
        public async Task<IActionResult> ConfirmPasswordReset(ConfirmPasswordResetRequest request)
        {
            try
            {
                await _userService.ConfirmPasswordResetAsync(request);
                return Ok(new { message = "Пароль успешно изменен" });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}