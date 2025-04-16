
namespace RouletteTechTest.API.Models
{
    public static class RouletteColors
    {
        // Números rojos en la ruleta europea
        public static readonly HashSet<int> RedNumbers = new HashSet<int>
        {
            1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36
        };

        // Números negros en la ruleta europea
        public static readonly HashSet<int> BlackNumbers = new HashSet<int>
        {
            2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35
        };

        public static string GetColorForNumber(int number)
        {
            if (number == 0) return "verde";
            return RedNumbers.Contains(number) ? "rojo" : "negro";
        }

        public static string NormalizeColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return "";
            
            return color.ToLower() switch
            {
                "red" or "rojo" => "rojo",
                "black" or "negro" => "negro",
                "green" or "verde" => "verde",
                _ => color.ToLower()
            };
        }

        public static bool IsValidColorForNumber(int number, string color)
        {
            if (string.IsNullOrEmpty(color)) return false;
            
            string normalizedColor = NormalizeColor(color);
            string actualColor = GetColorForNumber(number);
            
            return string.Equals(normalizedColor, actualColor, StringComparison.OrdinalIgnoreCase);
        }
    }
}