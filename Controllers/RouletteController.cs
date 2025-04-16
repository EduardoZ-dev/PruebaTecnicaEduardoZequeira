using Microsoft.AspNetCore.Mvc;
using RouletteTechTest.API.Models;
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

        [HttpGet("spin")]
        public ActionResult<SpinResult> Spin()
        {
            var result = _rouletteService.Spin();
            return Ok(result);
        }

        [HttpPost("bet")]
        public async Task<ActionResult<BetResponse>> Bet([FromBody] BetRequest betRequest)
        {
            if (betRequest == null)
                return BadRequest("Solicitud de apuesta inválida.");

            var betResult = await _rouletteService.ProcessBet(betRequest);
            return Ok(betResult);
        }
    }
}
