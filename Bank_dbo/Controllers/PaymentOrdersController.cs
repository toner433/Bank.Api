using System;
using System.Threading.Tasks;
using Bank.Application.DTOs.PaymentOrders;
using Bank.Application.Services.Interfaces;
using Bank.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentOrdersController : ControllerBase
    {
        private readonly IPaymentOrderService _paymentOrderService;

        public PaymentOrdersController(IPaymentOrderService paymentOrderService)
        {
            _paymentOrderService = paymentOrderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentOrderRequest request)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var dto = await _paymentOrderService.CreateAsync(request, uid.Value);
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

        [HttpGet("organization/{organizationId:guid}")]
        public async Task<IActionResult> ListByOrganization(Guid organizationId)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var list = await _paymentOrderService.ListByOrganizationAsync(organizationId, uid.Value);
                return Ok(list);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id:guid}/execute")]
        public async Task<IActionResult> Execute(Guid id)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var dto = await _paymentOrderService.ExecuteAsync(id, uid.Value);
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
