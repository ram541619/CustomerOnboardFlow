using CustomerOnboard.Application.Services;
using CustomerOnboarding.Core.Entities;
using CustomerOnboarding.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerOnboard.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _service;

        public CustomersController(CustomerService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await _service.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound(new { Message = "Customer not found." });
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var (isCreated, errors) = await _service.AddCustomerAsync(customer);

            if (!isCreated)
                return BadRequest(new { Errors = errors });

            return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest updatedCustomer)
        {
            var existingCustomer = await _service.GetCustomerByIdAsync(id);
            if (existingCustomer == null)
                return NotFound(new { Message = "Account not found." });
            existingCustomer.Name = !string.IsNullOrEmpty(updatedCustomer.Name)
                ? updatedCustomer.Name
                : existingCustomer.Name;

            existingCustomer.Email = !string.IsNullOrEmpty(updatedCustomer.Email)
                ? updatedCustomer.Email
                : existingCustomer.Email;

            existingCustomer.ICNumber = !string.IsNullOrEmpty(updatedCustomer.ICNumber)
                ? updatedCustomer.ICNumber
                : existingCustomer.ICNumber;

            existingCustomer.MobileNumber = !string.IsNullOrEmpty(updatedCustomer.MobileNumber)
                ? updatedCustomer.MobileNumber
                : existingCustomer.MobileNumber;

            existingCustomer.Pin = !string.IsNullOrEmpty(updatedCustomer.Pin)
                ? updatedCustomer.Pin
                : existingCustomer.Pin;

            if (updatedCustomer.HasPrivacyPolicyAgreed)
                existingCustomer.HasPrivacyPolicyAgreed = updatedCustomer.HasPrivacyPolicyAgreed;

            await _service.UpdateCustomerAsync(existingCustomer);
            return Ok(new { message = "Updated successfully" });
        }


        [HttpPost("validate-login")]
        public async Task<IActionResult> ValidateLogin([FromBody] string icNumber)
        {
            if (string.IsNullOrEmpty(icNumber))
            {
                return BadRequest("IC Number is required");
            }

            var (validAccount, error) = await _service.ValidateLoginAsync(icNumber);

            if (!validAccount)
            {
                return NotFound(new { message = error });
            }

            return Ok(new { message = "OTP sent successfully" });
        }


        [HttpPost("generate-otp")]
        public async Task<IActionResult> GenerateOTP([FromBody] OTPSentRequest contactInfo)
        {
            var (isGenerated, otp, error) = await _service.GenerateOTPAsync(contactInfo.ContactInfo, contactInfo.ContactType == "Mobile");
            if (!isGenerated)
                return BadRequest(new { Error = error });
            return Ok(new { Message = "OTP sent successfully.", OTP = otp });
        }

        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOTP([FromBody] OTPValidationRequest request)
        {
            var (isValid, error) = await _service.ValidateOTPAsync(request.ContactInfo, request.OTP, request.ContactType == "Mobile");
            if (!isValid)
                return BadRequest(new { Error = error });
            return Ok(new { Message = "OTP validated successfully." });
        }

        [HttpPost("{id}/enable-biometric")]
        public async Task<IActionResult> EnableBiometricLogin(int id, [FromBody] BiometricRequest request)
        {
            Customer user = await _service.GetCustomerByIdAsync(id);
            if (user == null)
                return NotFound(new { error = "Account not found." });

            user.IsEnableBiometric = request.EnableBiometric;
            await _service.UpdateCustomerAsync(user);

            return Ok(new { message = "Biometric login enabled successfully." });
        }

        [HttpGet("{id}/dashboard")]
        public async Task<IActionResult> GetDashboard(int id)
        {
            var user = await _service.GetCustomerByIdAsync(id);
            if (user == null)
                return NotFound(new { error = "Account not found." });
                    var dashboard = new
                    {
                        name = user.Name,
                        personalFinance = new
                        {
                            message = "Build your home. Together.",
                            description = "Realize your dream with our exciting financing with competitive rates."
                        },
                        offers = new[]
                        {
                        new
                        {
                              title = "Oh My Cashback!",
                              description = "Stand a chance to win cash prize with our financing!"
                        }
                   }
            };
            return Ok(dashboard);
        }


    }
}
