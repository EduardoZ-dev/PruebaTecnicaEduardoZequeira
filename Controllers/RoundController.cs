using Microsoft.AspNetCore.Mvc;
using RouletteTechTest.API.Models.DTOs.Bet;
using RouletteTechTest.API.Models.DTOs.Round;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoundsController : ControllerBase
    {
        private readonly IRoundService _roundService;

        public RoundsController(IRoundService roundService)
        {
            _roundService = roundService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRounds()
        {
            try
            {
                var rounds = await _roundService.GetAllRoundAsync();

                var result = rounds.Select(r => new
                {
                    r.Id,
                    r.RoundNumber,
                    r.StartTime,
                    r.EndTime,
                    IsClosed = r.EndTime.HasValue,
                    Result = r.Result != null ? new
                    {
                        r.Result.ResultNumber,
                        r.Result.Color,
                        r.Result.Parity,
                        r.Result.SpinTime
                    } : null,
                    SessionId = r.SessionId,
                    TotalBets = r.Bets?.Count ?? 0,
                    Bets = r.Bets.Select(b => new BetDTO
                    {
                        Id = b.Id,
                        UserName = b.UserName != null ? b.UserName : "N/A",
                        BetType = b.BetType.ToString(),
                        BetValue = b.BetValue,
                        Amount = b.Amount,
                        TimeStamp = b.TimeStamp,
                        Outcome = b.Outcome.ToString(),
                        Prize = b.Prize
                    }).ToList()
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartRound([FromBody] StartRoundRequest request)
        {
            try
            {
                var newRound = await _roundService.StartRoundAsync(request.SessionId);
                return Ok(new
                {
                    RoundId = newRound.Id,
                    newRound.StartTime,
                    SessionId = newRound.SessionId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("{roundId}/close")]
        public async Task<IActionResult> CloseRound(Guid roundId)
        {
            try
            {
                await _roundService.CloseRoundAsync(roundId);
                return NoContent(); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{roundId}")]
        public async Task<IActionResult> GetRoundDetails(Guid roundId)
        {
            try
            {
                var round = await _roundService.GetRoundDetailsAsync(roundId);
                if (round == null)
                    return NotFound("Ronda no encontrada");

                return Ok(new
                {
                    round.Id,
                    round.StartTime,
                    round.EndTime,

                    Result = round.Result != null ? new
                    {
                        round.Result.ResultNumber,
                        round.Result.Color,
                        round.Result.Parity,
                        round.Result.SpinTime
                    } : null,
                    TotalBets = round.Bets?.Count ?? 0
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetRoundsBySession(Guid sessionId)
        {
            try
            {
                var rounds = await _roundService.GetRoundsBySessionAsync(sessionId);
                return Ok(rounds.Select(r => new
                {
                    r.Id,
                    r.StartTime,
                    r.EndTime,
                    IsClosed = r.EndTime.HasValue,
                    Result = r.Result != null ? new
                    {
                        r.Result.ResultNumber,
                        r.Result.Color,
                        r.Result.Parity,
                        r.Result.SpinTime
                    } : null,
                    TotalBets = r.Bets?.Count ?? 0
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
