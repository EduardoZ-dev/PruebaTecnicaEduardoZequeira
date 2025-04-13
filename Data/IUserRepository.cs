using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Data
{
    public interface IUserRepository
    {
        Task<User?> GetUserByNameAsync(string userName);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task SaveChangesAsync();
    }
}
