using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerOnboarding.Core
{
    public static class Constants
    {
        public const int OTP_EXPIRY_MINUTES = 5;
        public const int OTP_MIN = 1000;
        public const int OTP_MAX = 9999;
        public const string ACCOUNT_EXISTS = "Account already exists";
        public const string ACCOUNT_NOT_EXIST = "Account does not exist";
        public static string ACCOUNT_NOT_VERIFIED = "Account not verified yet";
        public static string OTP_SENT = "OTP sent successfully";
        public static string CUSTOMER_NOT_FOUND = "Customer not found.";
        public static string MOBILE_OTP_SENT = "Mobile OTP has expired or is invalid.";
        public static string EMAIL_OTP_SENT = "Email OTP has expired or is invalid.";
        public static string INCORRECT_MOBILE_OTP = "Incorrect Mobile OTP.";
        public static string INCORRECT_EMAIL_OTP = "Incorrect Email OTP.";
    }
}
