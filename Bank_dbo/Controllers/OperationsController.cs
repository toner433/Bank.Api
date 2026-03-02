using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bank.Application.Services.Interfaces;
using OperationFilterDto = Bank.Application.DTOs.Operations.OperationFilterDto;

namespace Bank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OperationsController : ControllerBase
    {
        private readonly IOperationService _operationService;

        public OperationsController(IOperationService operationService)
        {
            _operationService = operationService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOperations(
            Guid userId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int? limit)
        {
            var filter = new OperationFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate,
                Limit = limit
            };

            var operations = await _operationService.GetUserOperationsAsync(userId, filter);
            return Ok(operations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var operation = await _operationService.GetOperationByIdAsync(id);
            if (operation == null)
                return NotFound();
            return Ok(operation);
        }
    }
}