using System;
using System.Threading.Tasks;
using Bank.Application.DTOs.Organizations;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterOrganizationRequest request)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var dto = await _organizationService.RegisterAsync(request, uid.Value);
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
        public async Task<IActionResult> MyOrganizations()
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            var list = await _organizationService.GetMyOrganizationsAsync(uid.Value);
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            var dto = await _organizationService.GetByIdAsync(id, uid.Value);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpGet("{id:guid}/members")]
        public async Task<IActionResult> Members(Guid id)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var list = await _organizationService.GetMembersAsync(id, uid.Value);
                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id:guid}/members")]
        public async Task<IActionResult> AddMember(Guid id, [FromBody] AddOrganizationMemberRequest request)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var dto = await _organizationService.AddMemberAsync(id, request, uid.Value);
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

        [HttpDelete("{id:guid}/members/{userId:guid}")]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                await _organizationService.RemoveMemberAsync(id, userId, uid.Value);
                return Ok(new { success = true });
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
