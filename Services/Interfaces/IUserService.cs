using RouletteTechTest.API.Models.DTOs.Common;
using RouletteTechTest.API.Models.DTOs.User;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<UserResponseDTO> UpdateUserNameAsync(Guid userId, string newUserName);
        Task<UserResponseDTO> UpdateUserBalanceByIdAsync(Guid userId, decimal amount);
        Task<UserResponseDTO> SaveOrUpdateUserBalanceAsync(SaveBalanceDTO dto);
        Task DeleteUserAsync(Guid id);
        Task AdjustUserBalanceAsync(Guid userId, decimal amount);

    }
}