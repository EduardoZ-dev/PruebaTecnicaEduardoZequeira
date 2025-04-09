using Microsoft.AspNetCore.Mvc;
using RouletteTechTest.API.Models.DTOs.Session;
using RouletteTechTest.API.Services.Interfaces;

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

        [HttpGet]
        public async Task<IActionResult> GetAllSessions()
        {
            var sessions = await _sessionService.GetAllSessionsAsync();
            return Ok(sessions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSessionById(Guid id)
        {
            try
            {
                var session = await _sessionService.GetSessionByIdAsync(id);
                return Ok(session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] SessionCreateDTO createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var session = await _sessionService.CreateSessionAsync(createDto);
                return CreatedAtAction(
                    nameof(GetSessionById),
                    new { id = session.Id },
                    session
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Error interno del servidor",
                    Details = ex.Message
                });
            }
        }

        [HttpPost("{sessionId}/end")]
        public async Task<IActionResult> EndSession(Guid sessionId)
        {
            try
            {
                await _sessionService.EndSessionAsync(sessionId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
