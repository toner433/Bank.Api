using Bank.Application.DTOs.Accounts;
using Bank.Application.DTOs.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Services.Interfaces
{
    public interface IOperationService
    {
        Task<OperationDto> TransferAsync(TransferRequest request);
        Task<OperationDto?> GetOperationByIdAsync(Guid id);
        Task<List<OperationDto>> GetUserOperationsAsync(Guid userId, OperationFilterDto filter);
        
    }
}
