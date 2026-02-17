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
        private readonly BankDbContext _context;

        public OperationTypeRepository(BankDbContext context)
        {
            _context = context;
        }

        public async Task<OperationType?> GetByIdAsync(Guid id)
        {
            return await _context.OperationTypes.FindAsync(id);
        }

        public async Task<OperationType?> GetByNameAsync(string name)
        {
            return await _context.OperationTypes
                .FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<List<OperationType>> GetAllAsync()
        {
            return await _context.OperationTypes.ToListAsync();
        }
    }
}
