using Bank.Domain.Interfaces;
using Bank.Domain.Models;
using Bank.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Repositories
{
    public class OperationTypeRepository : IOperationTypeRepository
    {
        private readonly IDataBaseRepository _repository;

        public OperationTypeRepository(IDataBaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationType?> GetByNameAsync(string name)
        {
            var types = await _repository.GetAllAsync<OperationType>();
            return types.FirstOrDefault(t => t.Name == name);
        }
    }
}
