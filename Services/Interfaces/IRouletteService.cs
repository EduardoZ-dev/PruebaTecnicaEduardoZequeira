using RouletteTechTest.API.Models.DTOs.Common;

public interface IRouletteService
{
    Task<SpinResultDTO> SpinAsync(Guid sessionId);
}