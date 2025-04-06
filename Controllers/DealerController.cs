using Microsoft.AspNetCore.Mvc;
using RouletteTechTest.API.Models.DTOs;
using RouletteTechTest.API.Models.Enums;
using RouletteTechTest.API.Services.Interfaces;

namespace RouletteTechTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DealerController : ControllerBase
    {
        private readonly IBetService _betService;
        private readonly IBetCalculator _betCalculator;
        private readonly IUserService _userService;

        public DealerController(
            IBetService betService,
            IBetCalculator betCalculator,
            IUserService userService)
        {
            _betService = betService;
            _betCalculator = betCalculator;
            _userService = userService;
        }

        [HttpPost("process-bet")]
        public async Task<IActionResult> ProcessBet([FromBody] DealerRequestDTO request)
        {
            // 1. Recuperar la apuesta por ID
            var bet = await _betService.GetBetByIdAsync(request.BetId);
            if (bet == null)
                return NotFound(new { Error = "Apuesta no encontrada." });

            // 2. Calcular el resultado de la apuesta usando el BetCalculator
            var result = _betCalculator.CalculateResult(bet, request.WinningNumber);
            bet.Outcome = result.Outcome;
            bet.Prize = result.Prize;

            // 3. Actualizar la apuesta en base de datos
            await _betService.UpdateBetAsync(bet);

            // 4. Ajustar el balance del usuario:
            //     Si gana, se suma el premio; si pierde, se resta el monto apostado.
            decimal balanceChange = (bet.Outcome == BetOutcome.Win) ? result.Prize : -bet.Amount;
            await _userService.AdjustUserBalanceAsync(bet.UserId, balanceChange);

            // 5. Retornar el resultado final, incluyendo el nuevo balance del usuario
            var user = await _userService.GetUserByIdAsync(bet.UserId);
            return Ok(new
            {
                BetId = bet.Id,
                Outcome = bet.Outcome.ToString(),
                Prize = bet.Prize,
                NewBalance = user?.Balance
            });
        }
    }
}
