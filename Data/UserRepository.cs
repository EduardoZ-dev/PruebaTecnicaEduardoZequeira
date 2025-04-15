using Microsoft.EntityFrameworkCore;
using RouletteTechTest.API.Data.Context;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByNameAsync(string userName)
        {
            // Normalizar el nombre a minúsculas para búsqueda case-insensitive
            string normalizedName = userName.ToLower();
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == normalizedName);
        }

        public async Task AddUserAsync(User user)
        {
            // Normalizar el nombre antes de agregar
            user.UserName = user.UserName.ToLower();
            await _context.Users.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            // Guarda todos los cambios en la base de datos.
            await _context.SaveChangesAsync();
        }
    }
}
