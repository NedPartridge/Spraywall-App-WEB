using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpraywallAppWeb.Migrations
{
    /// <inheritdoc />
    public partial class WallRelations1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "ManagerID",
                table: "Walls",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Walls_ManagerID",
                table: "Walls",
                column: "ManagerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Walls_User_ManagerID",
                table: "Walls",
                column: "ManagerID",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Walls_User_ManagerID",
                table: "Walls");

            migrationBuilder.DropIndex(
                name: "IX_Walls_ManagerID",
                table: "Walls");

            migrationBuilder.DropColumn(
                name: "ManagerID",
                table: "Walls");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Name", "Password" },
                values: new object[] { 1, "jeff.theman@coolguy.com", "Jeff", "Password0!" });
        }
    }
}
