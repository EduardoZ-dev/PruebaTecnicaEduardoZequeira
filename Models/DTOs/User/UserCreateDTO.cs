namespace RouletteTechTest.API.Models.DTOs.User
{
    public class UserCreateDTO
    {
        public string UserName { get; set; } = null!;
        public decimal InitialBalance { get; set; }
    }
}
