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

        public async Task<List<AdminUserListItemDto>> ListUsersAsync(Guid actingUserId, AdminUserFilterRequest? filter = null)
        {
            await EnsureAdminAsync(actingUserId);
            var users = await _db.GetAllAsync<User>();
            
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "all")
                {
                    users = filter.Status switch
                    {
                        "blocked" => users.Where(x => x.IsBlocked).ToList(),
                        "active" => users.Where(x => !x.IsBlocked).ToList(),
                        _ => users
                    };
                }
                
                if (!string.IsNullOrEmpty(filter.Role) && filter.Role != "all")
                {
                    users = filter.Role switch
                    {
                        "admin" => users.Where(x => x.IsAdmin).ToList(),
                        "client" => users.Where(x => !x.IsAdmin).ToList(),
                        _ => users
                    };
                }
                
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var search = filter.Search.ToLower().Trim();
                    users = users.Where(x => 
                        x.Login.ToLower().Contains(search) ||
                        x.FullName.ToLower().Contains(search) ||
                        x.Email.ToLower().Contains(search)
                    ).ToList();
                }
            }
            
            var sortBy = filter?.SortBy ?? "createdAt";
            var sortOrder = filter?.SortOrder ?? "desc";
            
            var sorted = sortBy switch
            {
                "login" => sortOrder == "asc" 
                    ? users.OrderBy(x => x.Login) 
                    : users.OrderByDescending(x => x.Login),
                "fullName" => sortOrder == "asc" 
                    ? users.OrderBy(x => x.FullName) 
                    : users.OrderByDescending(x => x.FullName),
                "email" => sortOrder == "asc" 
                    ? users.OrderBy(x => x.Email) 
                    : users.OrderByDescending(x => x.Email),
                _ => sortOrder == "asc" 
                    ? users.OrderBy(x => x.CreatedAt) 
                    : users.OrderByDescending(x => x.CreatedAt)
            };
            
            return sorted
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

        public async Task<List<AdminOrganizationListItemDto>> ListOrganizationsAsync(Guid actingUserId, AdminOrganizationFilterRequest? filter = null)
        {
            await EnsureAdminAsync(actingUserId);
            var orgs = await _db.GetAllAsync<Organization>();
            
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var search = filter.Search.ToLower().Trim();
                    orgs = orgs.Where(x => 
                        x.Name.ToLower().Contains(search) ||
                        x.Inn.ToLower().Contains(search)
                    ).ToList();
                }
            }
            
            var sortBy = filter?.SortBy ?? "createdAt";
            var sortOrder = filter?.SortOrder ?? "desc";
            
            var sorted = sortBy switch
            {
                "name" => sortOrder == "asc" 
                    ? orgs.OrderBy(x => x.Name) 
                    : orgs.OrderByDescending(x => x.Name),
                "inn" => sortOrder == "asc" 
                    ? orgs.OrderBy(x => x.Inn) 
                    : orgs.OrderByDescending(x => x.Inn),
                _ => sortOrder == "asc" 
                    ? orgs.OrderBy(x => x.CreatedAt) 
                    : orgs.OrderByDescending(x => x.CreatedAt)
            };
            
            return sorted
                .Select(x => new AdminOrganizationListItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Inn = x.Inn,
                    CreatedAt = x.CreatedAt
                })
                .ToList();
        }

        public async Task<List<AdminAccountListItemDto>> ListAccountsAsync(Guid actingUserId, AdminAccountFilterRequest? filter = null)
        {
            await EnsureAdminAsync(actingUserId);
            var accounts = await _db.GetAllAsync<Account>();
            var users = await _db.GetAllAsync<User>();
            var orgs = await _db.GetAllAsync<Organization>();
            
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Currency) && filter.Currency != "all")
                {
                    accounts = accounts.Where(x => x.Currency == filter.Currency).ToList();
                }
                
                if (!string.IsNullOrEmpty(filter.AccountType) && filter.AccountType != "all")
                {
                    accounts = accounts.Where(x => x.AccountType == filter.AccountType).ToList();
                }
                
                if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "all")
                {
                    accounts = filter.Status switch
                    {
                        "blocked" => accounts.Where(x => x.IsBlocked).ToList(),
                        "active" => accounts.Where(x => !x.IsBlocked).ToList(),
                        _ => accounts
                    };
                }
                
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var search = filter.Search.ToLower().Trim();
                    accounts = accounts.Where(x => 
                        x.AccountNumber.ToLower().Contains(search) ||
                        (x.UserId.HasValue && users.FirstOrDefault(u => u.Id == x.UserId.Value)?.FullName.ToLower().Contains(search) == true) ||
                        (x.OrganizationId.HasValue && orgs.FirstOrDefault(o => o.Id == x.OrganizationId.Value)?.Name.ToLower().Contains(search) == true)
                    ).ToList();
                }
            }
            
            var sortBy = filter?.SortBy ?? "createdAt";
            var sortOrder = filter?.SortOrder ?? "desc";
            
            var sorted = sortBy switch
            {
                "accountNumber" => sortOrder == "asc" 
                    ? accounts.OrderBy(x => x.AccountNumber) 
                    : accounts.OrderByDescending(x => x.AccountNumber),
                "balance" => sortOrder == "asc" 
                    ? accounts.OrderBy(x => x.Balance) 
                    : accounts.OrderByDescending(x => x.Balance),
                _ => sortOrder == "asc" 
                    ? accounts.OrderBy(x => x.CreatedAt) 
                    : accounts.OrderByDescending(x => x.CreatedAt)
            };
            
            return sorted
                .Select(x => new AdminAccountListItemDto
                {
                    Id = x.Id,
                    AccountNumber = x.AccountNumber,
                    Balance = x.Balance,
                    Currency = x.Currency,
                    AccountType = x.AccountType,
                    IsBlocked = x.IsBlocked,
                    AdminComment = x.AdminComment,
                    Owner = x.UserId.HasValue
                        ? users.FirstOrDefault(u => u.Id == x.UserId.Value)?.FullName ?? "Пользователь"
                        : (x.OrganizationId.HasValue
                            ? orgs.FirstOrDefault(o => o.Id == x.OrganizationId.Value)?.Name ?? "Организация"
                            : "Не определён"),
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

        public async Task SetAccountBlockedAsync(Guid actingUserId, Guid accountId, bool blocked)
        {
            await EnsureAdminAsync(actingUserId);
            var account = await _db.GetByIdAsync<Account>(accountId);
            if (account == null) throw new NotFoundException("Счёт не найден");
            account.IsBlocked = blocked;
            await _db.UpdateAsync(account);
        }

        public async Task UpdateAccountAsync(Guid actingUserId, Guid accountId, UpdateAdminAccountRequest request)
        {
            await EnsureAdminAsync(actingUserId);
            var account = await _db.GetByIdAsync<Account>(accountId);
            if (account == null) throw new NotFoundException("Счёт не найден");
            account.AdminComment = string.IsNullOrWhiteSpace(request.AdminComment) ? null : request.AdminComment.Trim();
            if (request.Balance.HasValue)
            {
                if (request.Balance.Value < 0) throw new BusinessException("Баланс не может быть отрицательным");
                account.Balance = request.Balance.Value;
            }
            if (!string.IsNullOrWhiteSpace(request.Currency))
                account.Currency = request.Currency.Trim().ToUpper();
            if (!string.IsNullOrWhiteSpace(request.AccountType))
                account.AccountType = request.AccountType.Trim();
            if (request.IsBlocked.HasValue)
                account.IsBlocked = request.IsBlocked.Value;
            await _db.UpdateAsync(account);
        }
    }
}
