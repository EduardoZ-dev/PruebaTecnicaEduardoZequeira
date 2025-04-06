using RouletteTechTest.API.Data.Common;
using RouletteTechTest.API.Models.DTOs.Common;
using RouletteTechTest.API.Models.DTOs.User;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        private UserResponseDTO MapToResponseDTO(User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                Name = user.UserName,
                Balance = user.Balance
            };
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            return await _uow.Users.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _uow.Users.GetByIdAsync(id);
        }

        public async Task<UserResponseDTO> UpdateUserNameAsync(Guid userId, string newUserName)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(newUserName))
                    throw new ArgumentException("El nombre de usuario no puede estar vacío.");

                var existingUser = await _uow.Users.GetByNameAsync(newUserName.ToLower());
                if (existingUser != null && existingUser.Id != userId)
                    throw new InvalidOperationException($"El nombre '{newUserName}' ya está en uso.");

                // Actualizar usando repositorio
                await _uow.Users.UpdateUserNameAsync(userId, newUserName.Trim());

                // Guardar cambios
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                var updatedUser = await _uow.Users.GetByIdAsync(userId);
                return MapToResponseDTO(updatedUser);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                await _uow.Users.DeleteAsync(id);
                await _uow.SaveChangesAsync(); 
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<UserResponseDTO> SaveOrUpdateUserBalanceAsync(SaveBalanceDTO dto)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var normalizedUserName = dto.UserName.Trim().ToLower();
                var user = await _uow.Users.GetByNameAsync(normalizedUserName);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = dto.UserName.Trim(),
                        Balance = dto.Amount
                    };
                    await _uow.Users.AddAsync(user);
                }
                else
                {
                    user.Balance += dto.Amount;
                    _uow.Users.UpdateAsync(user);
                }

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return MapToResponseDTO(user);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<UserResponseDTO> UpdateUserBalanceByIdAsync(Guid userId, decimal amount)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var user = await _uow.Users.GetByIdAsync(userId)
                    ?? throw new KeyNotFoundException("Usuario no encontrado");

                user.Balance += amount;
                _uow.Users.UpdateAsync(user);

                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();

                return MapToResponseDTO(user);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task AdjustUserBalanceAsync(Guid userId, decimal amount)
        {
            // Buscar el usuario por su ID.
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            // Ajustar el balance (suma o resta, según el valor de amount)
            user.Balance += amount;

            // Actualizar el usuario a través del repositorio.
            await _uow.Users.UpdateBalanceAsync(userId, amount);
        }

    }
}