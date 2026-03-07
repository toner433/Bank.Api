using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bank.Application.DTOs.Users;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("by-login/{login}")]
        public async Task<IActionResult> GetByLogin(string login)
        {
            var user = await _userService.GetByLoginAsync(login);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(Guid id, UpdateProfileRequest request)
        {
            try
            {
                var result = await _userService.UpdateProfileAsync(id, request);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordRequest request)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(id, request);
                return Ok(new { success = result });
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

        [AllowAnonymous]
        [HttpGet("check-login/{login}")]
        public async Task<IActionResult> CheckLogin(string login)
        {
            var exists = await _userService.ExistsAsync(login);
            return Ok(new { login, isAvailable = !exists });
        }
    }
}