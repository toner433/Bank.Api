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
    public class OperationTypeRepository : BaseRepository<OperationType>, IOperationTypeRepository
    {
        public OperationTypeRepository(BankDbContext context) : base(context)
        {
        }

        public async Task<OperationType?> GetByNameAsync(string name)
        {
            return await _context.OperationTypes
                .FirstOrDefaultAsync(t => t.Name == name);
        }
    }
}
