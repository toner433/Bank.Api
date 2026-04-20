using Bank.Application.Common;

namespace Bank.Application.DTOs.Organizations
{
    public class AddOrganizationMemberRequest
    {
        public string UserLogin { get; set; } = string.Empty;
        public string Role { get; set; } = OrganizationRoles.Accountant;
    }
}
