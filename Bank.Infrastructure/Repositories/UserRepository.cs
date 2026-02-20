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
    public class UserRepository : IUserRepository
    {
        private readonly IDataBaseRepository _repository;

        public UserRepository(IDataBaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            var users = await _repository.GetAllAsync<User>();
            return users.FirstOrDefault(u => u.Login == login);
        }

        public async Task<bool> ExistsAsync(string login)
        {
            var users = await _repository.GetAllAsync<User>();
            return users.Any(u => u.Login == login);
        }
    }
}
