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
        private readonly IPdfDocumentService _pdfDocumentService;

        public PaymentOrdersController(IPaymentOrderService paymentOrderService, IPdfDocumentService pdfDocumentService)
        {
            _paymentOrderService = paymentOrderService;
            _pdfDocumentService = pdfDocumentService;
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
        public async Task<IActionResult> Execute(Guid id, [FromBody] ExecutePaymentOrderRequest request)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var dto = await _paymentOrderService.ExecuteAsync(id, request.DeviceDetected, uid.Value);
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

        [HttpPost("{id:guid}/sign")]
        public async Task<IActionResult> Sign(Guid id, [FromBody] SignPaymentOrderRequest request)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var dto = await _paymentOrderService.SignAsync(id, request, uid.Value);
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

        [HttpGet("{id:guid}/document.pdf")]
        public async Task<IActionResult> DownloadDocumentPdf(Guid id)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var pdf = await _pdfDocumentService.GeneratePaymentOrderPdfAsync(id, uid.Value);
                return File(pdf, "application/pdf", $"payment-order-{id}.pdf");
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
