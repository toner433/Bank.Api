using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank.Application.DTOs.Admin;
using Bank.Application.Exceptions;
using Bank.Application.Services.Interfaces;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;

namespace Bank.Application.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IDataBaseRepository _db;

        public AdminService(IDataBaseRepository db)
        {
            _db = db;
        }

        public async Task EnsureAdminAsync(Guid actingUserId)
        {
            var u = await _db.GetByIdAsync<User>(actingUserId);
            if (u == null || !u.IsAdmin)
                throw new BusinessException("Доступ только для администратора");
        }

        public async Task<AdminStatsDto> GetStatsAsync(Guid actingUserId)
        {
            await EnsureAdminAsync(actingUserId);
            var users = await _db.GetAllAsync<User>();
            var orgs = await _db.GetAllAsync<Organization>();
            var accs = await _db.GetAllAsync<Account>();
            return new AdminStatsDto
            {
                UsersCount = users.Count,
                BlockedUsersCount = users.Count(x => x.IsBlocked),
                OrganizationsCount = orgs.Count,
                AccountsCount = accs.Count
            };
        }

        public async Task<List<AdminUserListItemDto>> ListUsersAsync(Guid actingUserId)
        {
            await EnsureAdminAsync(actingUserId);
            var users = await _db.GetAllAsync<User>();
            return users
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AdminUserListItemDto
                {
                    Id = x.Id,
                    Login = x.Login,
                    FullName = x.FullName,
                    Email = x.Email,
                    IsBlocked = x.IsBlocked,
                    IsAdmin = x.IsAdmin,
                    CreatedAt = x.CreatedAt
                })
                .ToList();
        }

        public async Task<List<AdminOrganizationListItemDto>> ListOrganizationsAsync(Guid actingUserId)
        {
            await EnsureAdminAsync(actingUserId);
            var orgs = await _db.GetAllAsync<Organization>();
            return orgs
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AdminOrganizationListItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Inn = x.Inn,
                    CreatedAt = x.CreatedAt
                })
                .ToList();
        }

        public async Task SetUserBlockedAsync(Guid actingUserId, Guid targetUserId, bool blocked)
        {
            await EnsureAdminAsync(actingUserId);
            if (actingUserId == targetUserId)
                throw new BusinessException("Нельзя изменить блокировку для самого себя");
            var target = await _db.GetByIdAsync<User>(targetUserId);
            if (target == null) throw new NotFoundException("Пользователь не найден");
            if (target.IsAdmin)
                throw new BusinessException("Нельзя блокировать администратора");
            target.IsBlocked = blocked;
            await _db.UpdateAsync(target);
        }
    }
}
