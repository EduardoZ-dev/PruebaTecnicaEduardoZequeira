using Microsoft.AspNetCore.Mvc;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services;

namespace RouletteTechTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // POST: api/session/start
        [HttpPost("start")]
        public ActionResult<SessionGame> StartSession([FromBody] StartSessionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) || request.InitialBalance < 0)
            {
                return BadRequest("Solicitud de inicio de sesión inválida.");
            }

            var session = _sessionService.StartSession(request.UserName, request.InitialBalance);
            return Ok(session);
        }

        // POST: api/session/bet
        [HttpPost("bet")]
        public ActionResult<BetResult> ProcessBet([FromBody] SessionBetRequest request)
        {
            if (request == null || request.SessionId == Guid.Empty)
            {
                return BadRequest("Solicitud de apuesta inválida.");
            }
            var betResult = _sessionService.ProcessBet(request.SessionId, request.Bet);
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

            await _sessionService.SaveSessionAsync(request.SessionId);
            return Ok("Sesión guardada exitosamente y saldo actualizado.");
        }
    }

    public class StartSessionRequest
    {
        public string UserName { get; set; }
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
    }
}
