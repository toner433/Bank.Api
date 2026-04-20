using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bank.Application.Common;
using Bank.Application.DTOs.Organizations;
using Bank.Application.Exceptions;
using Bank.Application.Services.Interfaces;
using Bank.Domain.Interfaces;
using Bank.Domain.Models;

namespace Bank.Application.Services.Implementations
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IDataBaseRepository _db;
        private readonly IUserRepository _userRepository;

        public OrganizationService(IDataBaseRepository db, IUserRepository userRepository)
        {
            _db = db;
            _userRepository = userRepository;
        }

        public async Task<OrganizationDto> RegisterAsync(RegisterOrganizationRequest request, Guid founderUserId)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new BusinessException("Укажите наименование организации");
            if (string.IsNullOrWhiteSpace(request.Inn) || request.Inn.Length is < 9 or > 12)
                throw new BusinessException("ИНН должен содержать от 9 до 12 цифр");
            if (string.IsNullOrWhiteSpace(request.LegalAddress)) throw new BusinessException("Укажите юридический адрес");

            var orgs = await _db.GetAllAsync<Organization>();
            if (orgs.Any(o => o.Inn == request.Inn))
                throw new BusinessException("Организация с таким ИНН уже зарегистрирована");

            var founder = await _db.GetByIdAsync<User>(founderUserId);
            if (founder == null) throw new NotFoundException("Пользователь не найден");

            var org = new Organization
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Inn = request.Inn.Trim(),
                Kpp = string.IsNullOrWhiteSpace(request.Kpp) ? null : request.Kpp.Trim(),
                LegalAddress = request.LegalAddress.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _db.AddAsync(org);

            var director = new OrganizationMember
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                UserId = founderUserId,
                Role = OrganizationRoles.Director,
                JoinedAt = DateTime.UtcNow
            };
            await _db.AddAsync(director);

            return MapOrg(org);
        }

        public async Task<List<OrganizationDto>> GetMyOrganizationsAsync(Guid userId)
        {
            var members = await _db.GetAllAsync<OrganizationMember>();
            var orgIds = members.Where(m => m.UserId == userId).Select(m => m.OrganizationId).Distinct().ToHashSet();
            var orgs = await _db.GetAllAsync<Organization>();
            return orgs.Where(o => orgIds.Contains(o.Id)).Select(MapOrg).OrderBy(o => o.Name).ToList();
        }

        public async Task<OrganizationDto?> GetByIdAsync(Guid id, Guid actingUserId)
        {
            if (!await UserIsMemberAsync(id, actingUserId)) return null;
            var org = await _db.GetByIdAsync<Organization>(id);
            return org == null ? null : MapOrg(org);
        }

        public async Task<List<OrganizationMemberDto>> GetMembersAsync(Guid organizationId, Guid actingUserId)
        {
            if (!await UserIsMemberAsync(organizationId, actingUserId))
                throw new BusinessException("Нет доступа к организации");

            var members = (await _db.GetAllAsync<OrganizationMember>())
                .Where(m => m.OrganizationId == organizationId)
                .ToList();
            var result = new List<OrganizationMemberDto>();
            foreach (var m in members)
            {
                var u = await _db.GetByIdAsync<User>(m.UserId);
                result.Add(new OrganizationMemberDto
                {
                    Id = m.Id,
                    UserId = m.UserId,
                    UserLogin = u?.Login ?? "",
                    UserFullName = u?.FullName ?? "",
                    Role = m.Role,
                    JoinedAt = m.JoinedAt
                });
            }
            return result.OrderBy(x => x.UserLogin).ToList();
        }

        public async Task<OrganizationMemberDto> AddMemberAsync(Guid organizationId, AddOrganizationMemberRequest request, Guid actingUserId)
        {
            if (!await UserIsDirectorAsync(organizationId, actingUserId))
                throw new BusinessException("Только директор может добавлять сотрудников");

            if (request.Role != OrganizationRoles.Director && request.Role != OrganizationRoles.Accountant)
                throw new BusinessException("Роль должна быть Director или Accountant");

            var user = await _userRepository.GetByLoginAsync(request.UserLogin.Trim());
            if (user == null) throw new NotFoundException("Пользователь с таким логином не найден");

            var existing = (await _db.GetAllAsync<OrganizationMember>())
                .Any(m => m.OrganizationId == organizationId && m.UserId == user.Id);
            if (existing) throw new BusinessException("Пользователь уже в организации");

            var member = new OrganizationMember
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                UserId = user.Id,
                Role = request.Role,
                JoinedAt = DateTime.UtcNow
            };
            await _db.AddAsync(member);

            return new OrganizationMemberDto
            {
                Id = member.Id,
                UserId = user.Id,
                UserLogin = user.Login,
                UserFullName = user.FullName,
                Role = member.Role,
                JoinedAt = member.JoinedAt
            };
        }

        public async Task RemoveMemberAsync(Guid organizationId, Guid memberUserId, Guid actingUserId)
        {
            if (!await UserIsDirectorAsync(organizationId, actingUserId))
                throw new BusinessException("Только директор может удалять сотрудников");

            var members = (await _db.GetAllAsync<OrganizationMember>())
                .Where(m => m.OrganizationId == organizationId)
                .ToList();
            var target = members.FirstOrDefault(m => m.UserId == memberUserId);
            if (target == null) throw new NotFoundException("Сотрудник не найден");

            if (target.Role == OrganizationRoles.Director)
            {
                var directors = members.Count(m => m.Role == OrganizationRoles.Director);
                if (directors <= 1)
                    throw new BusinessException("Нельзя удалить единственного директора");
            }

            await _db.DeleteAsync<OrganizationMember>(target.Id);
        }

        public async Task<bool> UserIsDirectorAsync(Guid organizationId, Guid userId)
        {
            var members = await _db.GetAllAsync<OrganizationMember>();
            return members.Any(m => m.OrganizationId == organizationId && m.UserId == userId && m.Role == OrganizationRoles.Director);
        }

        public async Task<bool> UserIsMemberAsync(Guid organizationId, Guid userId)
        {
            var members = await _db.GetAllAsync<OrganizationMember>();
            return members.Any(m => m.OrganizationId == organizationId && m.UserId == userId);
        }

        private static OrganizationDto MapOrg(Organization o) => new()
        {
            Id = o.Id,
            Name = o.Name,
            Inn = o.Inn,
            Kpp = o.Kpp,
            LegalAddress = o.LegalAddress,
            CreatedAt = o.CreatedAt
        };
    }
}
