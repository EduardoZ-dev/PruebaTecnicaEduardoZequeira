using Microsoft.AspNetCore.Mvc;

namespace RouletteTechTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouletteController : ControllerBase
    {
        private static readonly Random _random = new Random();
        private static readonly HashSet<int> RedNumbers = new HashSet<int>
        {
            1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36
        };

        [HttpPost("place-bet")]
        public IActionResult PlaceBet([FromBody] RouletteBetRequest request)
        {
            // Validar la apuesta
            if (request.Number == null && string.IsNullOrEmpty(request.Color))
                return BadRequest("Debes apostar al menos a un número o un color");

            if (request.Number.HasValue && (request.Number.Value < 0 || request.Number.Value > 36))
                return BadRequest("El número debe estar entre 0 y 36");

            if (!string.IsNullOrEmpty(request.Color))
            {
                var color = request.Color.ToLower();
                if (color != "red" && color != "black")
                    return BadRequest("Color debe ser 'red' o 'black'");
            }

            // Generar resultado aleatorio
            var resultNumber = _random.Next(0, 37);
            var resultColor = GetColor(resultNumber);

            // Verificar ganador
            var win = true;

            if (request.Number.HasValue && request.Number.Value != resultNumber)
                win = false;

            if (!string.IsNullOrEmpty(request.Color) &&
                request.Color.ToLower() != resultColor.ToLower())
                win = false;

            return Ok(new
            {
                Result = new { Number = resultNumber, Color = resultColor },
                Win = win
            });
        }

        private string GetColor(int number)
        {
            if (number == 0) return "green";
            return RedNumbers.Contains(number) ? "red" : "black";
        }

        public class RouletteBetRequest
        {
            public int? Number { get; set; }
            public string Color { get; set; }
        }
    }
}
