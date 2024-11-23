namespace CustomerOnboarding.Core.Models
{
    public class UpdateCustomerRequest
    {
        public string? Name { get; set; }
        public string? ICNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? Pin { get; set; }
        public bool IsVerified { get; set; }
        public bool HasPrivacyPolicyAgreed { get; set; }
    }
}
