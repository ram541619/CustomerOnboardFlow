using System.ComponentModel.DataAnnotations;

namespace CustomerOnboarding.Core.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "IC Number is required")]
        public string ICNumber { get; set; }
        [Required(ErrorMessage = "Mobile Number is required")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        public string? Pin { get; set; }
        public bool IsVerified { get; set; }
        public string? MobileOTP { get; set; }
        public DateTime MobileOTPExpiry { get; set; }
        public string? EmailOTP { get; set; }
        public DateTime EmailOTPExpiry { get; set; }
        public bool HasPrivacyPolicyAgreed { get; set; }
        public bool IsEnableBiometric { get; set; } = false;
    }
}
