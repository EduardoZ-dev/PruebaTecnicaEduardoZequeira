using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
            => await _context.Users.FindAsync(id);

        public async Task<User> GetByDNIAsync(int dni)
            => await _context.Users.FirstOrDefaultAsync(u => u.DNI == dni);

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
