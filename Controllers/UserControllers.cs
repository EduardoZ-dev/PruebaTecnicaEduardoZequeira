using Microsoft.AspNetCore.Mvc;
using RouletteTechTest.API.Data;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/users/load/{userName}
        [HttpGet("load/{userName}")]
        public async Task<ActionResult<User>> LoadUser(string userName)
        {
            var user = await _userRepository.GetUserByNameAsync(userName);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }
            return Ok(user);
        }

        // POST: api/users/update-balance
        [HttpPost("update-balance")]
        public async Task<IActionResult> UpdateUserBalance([FromBody] UpdateBalanceRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName))
                return BadRequest("Solicitud inválida.");

            var user = await _userRepository.GetUserByNameAsync(request.UserName);
            if (user == null)
            {
                user = new User { UserName = request.UserName, Balance = request.Amount };
                await _userRepository.AddUserAsync(user);
            }
            else
            {
                user.Balance = request.Amount;
                await _userRepository.UpdateUserAsync(user);
            }
            await _userRepository.SaveChangesAsync();
            return Ok("Saldo actualizado correctamente.");
        }
    }

    public class UpdateBalanceRequest
    {
        public string UserName { get; set; }
        public decimal Amount { get; set; }
    }
}
