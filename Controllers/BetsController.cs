using Microsoft.AspNetCore.Mvc;
using RouletteTechTest.API.Models.DTOs.Bet;

namespace RouletteTechTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BetsController : ControllerBase
    {
        private readonly IBetService _betService;

        public BetsController(IBetService betService)
        {
            _betService = betService;
        }

        [HttpPost("place-bet")]
        public async Task<IActionResult> PlaceBet([FromBody] BetRequestDTO request)
        {
            try
            {
                var result = await _betService.ProcessBetAndAdjustBalanceAsync(request);

                return Ok(new
                {
                    Message = "Apuesta procesada exitosamente",
                    Result = new
                    {
                        result.WinningNumber,
                        result.Outcome,
                        result.Prize,
                        result.Balance,
                        result.BetAmount,
                        result.BetType,
                        result.BetValue
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Error = "No se pudo procesar la apuesta",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("round/{roundId}")]
        public async Task<IActionResult> GetBetsByRound(Guid roundId)
        {
            try
            {
                var bets = await _betService.GetBetsByRoundAsync(roundId);
                return Ok(bets.Select(b => new {
                    b.Id,
                    b.Amount,
                    b.Type,
                    b.BetValue,
                    b.Outcome,
                    b.Prize
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
