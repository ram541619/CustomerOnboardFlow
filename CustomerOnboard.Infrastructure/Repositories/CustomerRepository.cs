using CustomerOnboard.Infrastructure.Data;
using CustomerOnboarding.Core.Entities;
using CustomerOnboarding.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CustomerOnboard.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerRepository(CustomerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync() => await _context.Customer.ToListAsync();

        public async Task<Customer> GetByIdAsync(int id) => await _context.Customer.FindAsync(id);

        public async Task AddAsync(Customer customer)
        {
            await _context.Customer.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customer.Update(customer);
            await _context.SaveChangesAsync();
        }
    }
}
