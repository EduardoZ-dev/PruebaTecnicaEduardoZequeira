using RouletteTechTest.API.Models.DTOs.User;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
        Task<User?> GetByNameAsync(string name);
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task UpdateUserNameAsync(Guid userId, string newUserName);
        Task DeleteAsync(Guid id);
        Task UpdateBalanceAsync(Guid userId, decimal amount);

        Task<bool> HasActiveSessionAsync(Guid userId);
    }
}
