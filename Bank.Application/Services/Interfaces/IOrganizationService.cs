using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bank.Application.DTOs.Organizations;

namespace Bank.Application.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<OrganizationDto> RegisterAsync(RegisterOrganizationRequest request, Guid founderUserId);
        Task<List<OrganizationDto>> GetMyOrganizationsAsync(Guid userId);
        Task<OrganizationDto?> GetByIdAsync(Guid id, Guid actingUserId);
        Task<List<OrganizationMemberDto>> GetMembersAsync(Guid organizationId, Guid actingUserId);
        Task<OrganizationMemberDto> AddMemberAsync(Guid organizationId, AddOrganizationMemberRequest request, Guid actingUserId);
        Task RemoveMemberAsync(Guid organizationId, Guid memberUserId, Guid actingUserId);
        Task<bool> UserIsDirectorAsync(Guid organizationId, Guid userId);
        Task<bool> UserIsMemberAsync(Guid organizationId, Guid userId);
    }
}
