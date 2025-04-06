using Microsoft.EntityFrameworkCore;
using RouletteTechTest.API.Data.Context;
using RouletteTechTest.API.Models.DTOs.User;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => new UserResponseDTO
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Balance = u.Balance
                })
                .ToListAsync();
        }
        public async Task<User?> GetByNameAsync(string name)
        {
            var normalizedName = name.Trim().ToLower();
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.Trim().ToLower() == normalizedName);
        }

        public async Task<User?> GetByIdAsync(Guid id)
            => await _context.Users.FindAsync(id);

        public async Task AddAsync(User user)
        {
            var existingUser = await GetByNameAsync(user.UserName);
            if (existingUser == null)
            {
                await _context.Users.AddAsync(user);
            }
        }

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task UpdateUserNameAsync(Guid userId, string newUserName)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");

            user.UserName = newUserName.Trim();
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task DeleteAsync(Guid id)
        {
   
            var user = new User { Id = id };
            _context.Users.Remove(user);
        }
        public async Task UpdateBalanceAsync(Guid userId, decimal amount)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            user.Balance += amount;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task CreateOrUpdateAsync(User user)
        {
            var existingUser = await GetByNameAsync(user.UserName);
            if (existingUser == null)
            {
                await _context.Users.AddAsync(user);
            }
            else
            {
                existingUser.Balance += user.Balance; 
                _context.Entry(existingUser).State = EntityState.Modified; 
            }
        }

        public async Task<bool> HasActiveSessionAsync(Guid userId)
        {
            return await _context.Sessions
                .Include(s => s.Players)
                .AnyAsync(s => s.Players.Any(p => p.Id == userId) && s.EndTime == null);
        }
    }
}
