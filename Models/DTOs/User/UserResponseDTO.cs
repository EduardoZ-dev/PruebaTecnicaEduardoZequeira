namespace RouletteTechTest.API.Models.DTOs.User
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}
