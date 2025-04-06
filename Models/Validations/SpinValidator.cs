using RouletteTechTest.API.Models.DTOs.Common;

namespace RouletteTechTest.API.Models.Validations
{
    public class SpinValidator
    {
        public void Validate(SpinResultDTO spinResult)
        {
            // Validar número entre 0 y 36
            if (spinResult.ResultNumber < 0 || spinResult.ResultNumber > 36)
                throw new ArgumentException("El número debe estar entre 0 y 36.");

            // Validar color (red, black, green)
            if (spinResult.ResultNumber == 0 && spinResult.Color != "green")
                throw new ArgumentException("El número 0 debe ser verde.");
            else if (spinResult.ResultNumber != 0)
            {
                bool isRed = spinResult.ResultNumber % 2 == 1 && spinResult.ResultNumber >= 1 && spinResult.ResultNumber <= 9 ||
                            spinResult.ResultNumber % 2 == 0 && spinResult.ResultNumber >= 12 && spinResult.ResultNumber <= 18 ||
                            spinResult.ResultNumber % 2 == 1 && spinResult.ResultNumber >= 19 && spinResult.ResultNumber <= 27 ||
                            spinResult.ResultNumber % 2 == 0 && spinResult.ResultNumber >= 30 && spinResult.ResultNumber <= 36;

                string expectedColor = isRed ? "red" : "black";
                if (spinResult.Color != expectedColor)
                    throw new ArgumentException($"El color para el número {spinResult.ResultNumber} debe ser {expectedColor}.");
            }

            // Validar paridad (even/odd)
            if (spinResult.ResultNumber != 0)
            {
                string expectedParity = spinResult.ResultNumber % 2 == 0 ? "even" : "odd";
                if (spinResult.Parity != expectedParity)
                    throw new ArgumentException($"La paridad para el número {spinResult.ResultNumber} debe ser {expectedParity}.");
            }
            else if (spinResult.Parity != null)
                throw new ArgumentException("La paridad no aplica para el número 0.");
        }
    }
}
