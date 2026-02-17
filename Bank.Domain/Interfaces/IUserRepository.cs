using Bank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByLoginAsync(string login);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> ExistsAsync(string login);
    }
}
