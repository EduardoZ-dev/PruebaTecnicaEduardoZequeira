using System.Text.Json.Serialization;

namespace RouletteTechTest.API.Models.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BetType
    {
        Color, 
        ParImpar,  
        Numero,      
        NumeroColor 
    }
}
