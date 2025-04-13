using Microsoft.AspNetCore.Mvc;
using RouletteTechTest.API.Models.Entities;
using RouletteTechTest.API.Services;

namespace RouletteTechTest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouletteController : ControllerBase
    {
        private readonly IRouletteService _rouletteService;

        public RouletteController(IRouletteService rouletteService)
        {
            _rouletteService = rouletteService;
        }

        // GET: api/roulette/spin
        [HttpGet("spin")]
        public ActionResult<SpinResult> Spin()
        {
            var result = _rouletteService.Spin();
            return Ok(result);
        }

        [HttpPost("bet")]
        public async Task<ActionResult<BetResult>> Bet([FromBody] BetRequest betRequest)
        {
            if (betRequest == null)
                return BadRequest("Solicitud de apuesta inválida.");

            var betResult = await _rouletteService.ProcessBetAsync(betRequest);
            return Ok(betResult);
        }
    }
}
