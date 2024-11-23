namespace CustomerOnboarding.Core.Models
{
    public class OTPValidationRequest
    {
        public string ContactInfo { get; set; } 
        public string OTP { get; set; }
        public string ContactType { get; set; }
    }
}
