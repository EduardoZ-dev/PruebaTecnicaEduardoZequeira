using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Services
{
    public interface IRouletteService
    {
        SpinResult Spin();
        decimal CalculatePrize(BetRequest betRequest, SpinResult spinResult);
        // Método que en la versión “inmediata” actualiza el saldo del usuario en la BD.
        Task<BetResult> ProcessBetAsync(BetRequest betRequest);
        // Para actualizar saldo sin lógica de apuesta (por ejemplo, guardado explícito)
        Task UpdateUserBalanceAsync(string userName, decimal amount);
    }
}
