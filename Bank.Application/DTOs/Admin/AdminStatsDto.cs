namespace Bank.Application.DTOs.Admin
{
    public class AdminStatsDto
    {
        public int UsersCount { get; set; }
        public int BlockedUsersCount { get; set; }
        public int OrganizationsCount { get; set; }
        public int AccountsCount { get; set; }
    }
}
