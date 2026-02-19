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
        Task<User?> GetByLoginAsync(string login);
        Task<bool> ExistsAsync(string login);
    }
}
