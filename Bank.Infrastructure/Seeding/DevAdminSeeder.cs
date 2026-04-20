using System;
using System.Linq;
using System.Threading.Tasks;
using Bank.Domain.Models;
using Bank.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Seeding
{
    public static class DevAdminSeeder
    {
        /// <summary>Логин bankadmin / пароль Admin123! — только для разработки.</summary>
        public static async Task SeedAsync(BankDbContext db)
        {
            if (await db.Users.AnyAsync(u => u.Login == "bankadmin"))
                return;

            await db.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                Login = "bankadmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Email = "admin@bank.local",
                Phone = "0000000000",
                FullName = "Системный администратор",
                PassportNumber = "0000000000",
                BirthDate = new DateOnly(1990, 1, 1),
                IsBlocked = false,
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
    }
}
