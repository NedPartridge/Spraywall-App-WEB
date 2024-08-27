using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpraywallAppWeb.Migrations
{
    /// <inheritdoc />
    public partial class Walls2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentifiedHoldsJsonPath",
                table: "Walls",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentifiedHoldsJsonPath",
                table: "Walls");
        }
    }
}
