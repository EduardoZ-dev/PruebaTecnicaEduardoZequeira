using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByDNIAsync(int dni);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
