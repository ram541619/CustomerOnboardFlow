using System.Reflection.Metadata;
using CustomerOnboarding.Core;
using CustomerOnboarding.Core.Entities;
using CustomerOnboarding.Core.Interfaces;

namespace CustomerOnboard.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _repository; 
        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<(bool IsValid, string error)> ValidateCustomerAsync(Customer customer)
        {
            var existingCustomer = await CheckExistingCustomer(customer);
            
            if (existingCustomer)
                return (false, Constants.ACCOUNT_EXISTS);

            return (true, string.Empty);
        }

        private async Task<bool> CheckExistingCustomer(Customer customer)
        {
            var existingCustomers = await _repository.GetAllAsync();
            return existingCustomers.Any(c => c.ICNumber == customer.ICNumber || c.Email == customer.Email);
        }

        public async Task<(bool IsCreated, string Errors)> AddCustomerAsync(Customer customer)
        {
            var (isValid, errors) = await ValidateCustomerAsync(customer);
            if (!isValid) return (false, errors);

            await _repository.AddAsync(customer);

            var mobileOtpResult = await GenerateOTPAsync(customer.MobileNumber, true);
            if (!string.IsNullOrEmpty(mobileOtpResult.Error))
                return (false,  mobileOtpResult.Error );

            var emailOtpResult = await GenerateOTPAsync(customer.Email, false);
            if (!string.IsNullOrEmpty(emailOtpResult.Error))
                return (false, emailOtpResult.Error );

            return (true, string.Empty);
        }

        public async Task<(bool ValidAccount, string Error)> ValidateLoginAsync(string icNumber)
        {
            var customer = (await _repository.GetAllAsync()).FirstOrDefault(c => c.ICNumber == icNumber);
            
            if (customer == null)
                return (false, Constants.ACCOUNT_NOT_EXIST);
                
            if (!customer.IsVerified)
                return (false, Constants.ACCOUNT_NOT_VERIFIED);

            var mobileOtpResult = await GenerateOTPAsync(customer.MobileNumber, true);
            if (!string.IsNullOrEmpty(mobileOtpResult.Error))
                return (false, mobileOtpResult.Error);

            var emailOtpResult = await GenerateOTPAsync(customer.Email, false);
            if (!string.IsNullOrEmpty(emailOtpResult.Error))
                return (false, emailOtpResult.Error);

            return (true, Constants.OTP_SENT);
        }

        public async Task<(bool IsGenerated, string OTP, string Error)> GenerateOTPAsync(string contactInfo, bool isMobileOTP)
        {
            var customer = await GetCustomerByContact(contactInfo, isMobileOTP);
            if (customer == null)
                return (false, null, Constants.CUSTOMER_NOT_FOUND);

            var otp = GenerateRandomOTP();
            await UpdateCustomerOTP(customer, otp, isMobileOTP);

            return (true, otp, null);
        }

        private async Task<Customer> GetCustomerByContact(string contactInfo, bool isMobileOTP)
        {
            var customers = await _repository.GetAllAsync();
            return customers.FirstOrDefault(c => 
                isMobileOTP ? c.MobileNumber == contactInfo : c.Email == contactInfo);
        }

        private string GenerateRandomOTP()
        {
            return new Random().Next(Constants.OTP_MIN, Constants.OTP_MAX).ToString();
        }

        private async Task UpdateCustomerOTP(Customer customer, string otp, bool isMobileOTP)
        {
            if (isMobileOTP)
            {
                customer.MobileOTP = otp;
                customer.MobileOTPExpiry = DateTime.UtcNow.AddMinutes(Constants.OTP_EXPIRY_MINUTES);
            }
            else
            {
                customer.EmailOTP = otp;
                customer.EmailOTPExpiry = DateTime.UtcNow.AddMinutes(Constants.OTP_EXPIRY_MINUTES);
            }

            await _repository.UpdateCustomerAsync(customer);
        }

        public async Task<(bool IsValid, string Error)> ValidateOTPAsync(string contactInfo, string otp, bool isMobileOTP)
        {
            var customer = await GetCustomerByContact(contactInfo, isMobileOTP);
            if (customer == null)
                return (false, Constants.CUSTOMER_NOT_FOUND);

            var validationResult = ValidateOTPDetails(customer, otp, isMobileOTP);
            if (!validationResult.IsValid)
                return validationResult;

            await UpdateCustomerVerification(customer, isMobileOTP);
            return (true, null);
        }

        private (bool IsValid, string Error) ValidateOTPDetails(Customer customer, string otp, bool isMobileOTP)
        {
            if (isMobileOTP)
            {
                if (customer.MobileOTP == null || customer.MobileOTPExpiry < DateTime.UtcNow)
                    return (false, Constants.MOBILE_OTP_SENT);
                if (customer.MobileOTP != otp)
                    return (false, Constants.INCORRECT_MOBILE_OTP);
            }
            else
            {
                if (customer.EmailOTP == null || customer.EmailOTPExpiry < DateTime.UtcNow)
                    return (false, Constants.EMAIL_OTP_SENT);
                if (customer.EmailOTP != otp)
                    return (false, Constants.INCORRECT_EMAIL_OTP);
            }
            return (true, null);
        }

        private async Task UpdateCustomerVerification(Customer customer, bool isMobileOTP)
        {
            if (isMobileOTP)
            {
                customer.MobileOTP = null;
                customer.MobileOTPExpiry = DateTime.MinValue;
            }
            else
            {
                customer.EmailOTP = null;
                customer.EmailOTPExpiry = DateTime.MinValue;
            }

            customer.IsVerified = true;
            await _repository.UpdateCustomerAsync(customer);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync() => 
            await _repository.GetAllAsync();

        public async Task<Customer> GetCustomerByIdAsync(int id) => 
            await _repository.GetByIdAsync(id);

        public async Task UpdateCustomerAsync(Customer customer) => 
            await _repository.UpdateCustomerAsync(customer);
    }
}