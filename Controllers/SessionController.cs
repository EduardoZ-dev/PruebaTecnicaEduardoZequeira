using Microsoft.AspNetCore.Mvc;
using RouletteTechTest.API.Data;
using RouletteTechTest.API.Models;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services;

namespace RouletteTechTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IUserRepository _userRepository;
        private readonly IRouletteService _rouletteService;
        private readonly List<Guid> _sessions = new List<Guid>();

        public SessionController(ISessionService sessionService, IUserRepository userRepository, IRouletteService rouletteService)
        {
            _sessionService = sessionService;
            _userRepository = userRepository;
            _rouletteService = rouletteService;
        }


        [HttpPost("start")]
        public async Task<ActionResult<SessionGame>> StartSession([FromBody] StartSessionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName))
            {
                return BadRequest("Solicitud de inicio de sesión inválida.");
            }

            // Normalizar el nombre de usuario a minúsculas
            request.UserName = request.UserName.ToLower();

            // Verificar si el usuario existe
            var existingUser = await _userRepository.GetUserByNameAsync(request.UserName);
            decimal finalBalance;

            if (existingUser != null)
            {
                // Si el usuario existe, usar su balance existente
                finalBalance = existingUser.Balance;
            }
            else
            {
                // Si el usuario no existe, crear uno nuevo con balance 0
                var newUser = new User
                {
                    UserName = request.UserName,
                    Balance = 0
                };
                await _userRepository.AddUserAsync(newUser);
                await _userRepository.SaveChangesAsync();
                finalBalance = 0;
            }

            // Si se proporciona un balance inicial, agregarlo
            if (request.InitialBalance > 0)
            {
                finalBalance += request.InitialBalance;
                var user = existingUser ?? await _userRepository.GetUserByNameAsync(request.UserName);
                if (user != null)
                {
                    user.Balance = finalBalance;
                    await _userRepository.UpdateUserAsync(user);
                    await _userRepository.SaveChangesAsync();
                }
            }

            // Crear la sesión con el balance final
            var session = _sessionService.StartSession(request.UserName, finalBalance);
            return Ok(session);
        }


        [HttpPost("bet")]
        public async Task<ActionResult<BetResponse>> ProcessBet([FromBody]SessionBetRequest request)
        {

            if (request == null || request.SessionId == Guid.Empty)
            {
                return BadRequest("Solicitud de apuesta inválida.");
            }

            if (request.Bet == null)
            {
                return BadRequest("Apuesta inválida.");
            }

            // Asignar el SessionId al BetRequest
            request.Bet.SessionId = request.SessionId;

            var betResult = await _rouletteService.ProcessBet(request.Bet);
            return Ok(betResult);
        }

        // GET: api/session/history/{sessionId}
        [HttpGet("history/{sessionId}")]
        public ActionResult<SessionGame> GetSessionHistory(Guid sessionId)
        {
            var session = _sessionService.GetSession(sessionId);
            if (session == null)
                return NotFound("Sesión no encontrada.");
            return Ok(session);
        }

        // POST: api/session/save
        [HttpPost("save")]
        public async Task<IActionResult> SaveSession([FromBody] SaveSessionRequest request)
        {
            if (request == null || request.SessionId == Guid.Empty)
                return BadRequest("Solicitud inválida.");

            var session = _sessionService.GetSession(request.SessionId);
            if (session == null)
                return NotFound("Sesión no encontrada.");

            await _sessionService.SaveSessionAsync(request.SessionId, request.CurrentBalance);

            return Ok(new { 
                message = "Usuario y saldo guardados exitosamente",
                userName = session.UserName,
                balance = session.CurrentBalance
            });
        }
    }

    public class StartSessionRequest
    {
        public string UserName { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; }
    }

    public class SessionBetRequest
    {
        public Guid SessionId { get; set; }
        public BetRequest Bet { get; set; }
    }

    public class SaveSessionRequest
    {
        public Guid SessionId { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
