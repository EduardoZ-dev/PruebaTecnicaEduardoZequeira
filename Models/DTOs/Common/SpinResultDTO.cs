
using Microsoft.EntityFrameworkCore;

namespace RouletteTechTest.API.Models.DTOs.Common
{
    [Owned]
    public class SpinResultDTO
    {
        public int ResultNumber { get; set; }
        public string Color { get; set; } = null!;
        public string Parity { get; set; } = null!;
        public DateTime SpinTime { get; set; } = DateTime.UtcNow;
    }
}
