using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface IOperationTypeRepository
    {
        Task<OperationType?> GetByIdAsync(Guid id);
        Task<OperationType?> GetByNameAsync(string name);
        Task<List<OperationType>> GetAllAsync();
    }
}
