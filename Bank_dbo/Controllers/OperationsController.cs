using System;
using System.Threading.Tasks;
using Bank.Application.Services.Interfaces;
using OperationFilterDto = Bank.Application.DTOs.Operations.OperationFilterDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OperationsController : ControllerBase
    {
        private readonly IOperationService _operationService;
        private readonly IPdfDocumentService _pdfDocumentService;

        public OperationsController(IOperationService operationService, IPdfDocumentService pdfDocumentService)
        {
            _operationService = operationService;
            _pdfDocumentService = pdfDocumentService;
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserOperations(
            Guid userId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int? limit)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            if (userId != uid.Value)
                return BadRequest(new { error = "Можно смотреть только свои операции" });

            var filter = new OperationFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                Limit = limit
            };

            var operations = await _operationService.GetUserOperationsAsync(userId, filter);
            return Ok(operations);
        }

        [HttpGet("organization/{organizationId:guid}")]
        public async Task<IActionResult> GetOrganizationOperations(
            Guid organizationId,
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
                var operations = await _operationService.GetOrganizationOperationsAsync(organizationId, uid.Value, filter);
                return Ok(operations);
            }
            catch (Bank.Application.Exceptions.BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var operation = await _operationService.GetOperationByIdAsync(id);
            if (operation == null)
                return NotFound();
            return Ok(operation);
        }

        [HttpGet("{id:guid}/receipt.pdf")]
        public async Task<IActionResult> DownloadReceiptPdf(Guid id)
        {
            var uid = User.GetCurrentUserId();
            if (uid == null) return Unauthorized();
            try
            {
                var pdf = await _pdfDocumentService.GenerateOperationReceiptPdfAsync(id, uid.Value);
                return File(pdf, "application/pdf", $"operation-{id}.pdf");
            }
            catch (Bank.Application.Exceptions.NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Bank.Application.Exceptions.BusinessException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
