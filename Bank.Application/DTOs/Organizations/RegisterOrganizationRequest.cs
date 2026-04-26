namespace Bank.Application.DTOs.Organizations
{
    public class RegisterOrganizationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Inn { get; set; } = string.Empty;
        public string? Kpp { get; set; }
        public string LegalAddress { get; set; } = string.Empty;
        public string? PublicKeyPem { get; set; }
    }
}
