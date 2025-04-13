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
            // Retorna el primer usuario que coincide con el nombre, o null si no existe.
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task AddUserAsync(User user)
        {
            // Agrega el usuario al contexto.
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
