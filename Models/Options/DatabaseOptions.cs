namespace RouletteTechTest.API.Models.Options
{
    public class DatabaseOptions
    {
        public const string SectionName = "SqlServerConnection";

        public string ConnectionString { get; set; } = null!;
    }
}
