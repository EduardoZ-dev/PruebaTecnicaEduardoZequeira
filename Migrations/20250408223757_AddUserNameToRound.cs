using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouletteTechTest.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToRound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Rounds",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Rounds");
        }
    }
}
