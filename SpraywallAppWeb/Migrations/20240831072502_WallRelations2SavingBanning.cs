using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpraywallAppWeb.Migrations
{
    /// <inheritdoc />
    public partial class WallRelations2SavingBanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AccessibleViaSearch",
                table: "Walls",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "UserWall",
                columns: table => new
                {
                    BannedUsersId = table.Column<int>(type: "INTEGER", nullable: false),
                    BannedWallsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWall", x => new { x.BannedUsersId, x.BannedWallsId });
                    table.ForeignKey(
                        name: "FK_UserWall_User_BannedUsersId",
                        column: x => x.BannedUsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWall_Walls_BannedWallsId",
                        column: x => x.BannedWallsId,
                        principalTable: "Walls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWall1",
                columns: table => new
                {
                    SavedUsersId = table.Column<int>(type: "INTEGER", nullable: false),
                    SavedWallsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWall1", x => new { x.SavedUsersId, x.SavedWallsId });
                    table.ForeignKey(
                        name: "FK_UserWall1_User_SavedUsersId",
                        column: x => x.SavedUsersId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWall1_Walls_SavedWallsId",
                        column: x => x.SavedWallsId,
                        principalTable: "Walls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserWall_BannedWallsId",
                table: "UserWall",
                column: "BannedWallsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWall1_SavedWallsId",
                table: "UserWall1",
                column: "SavedWallsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserWall");

            migrationBuilder.DropTable(
                name: "UserWall1");

            migrationBuilder.DropColumn(
                name: "AccessibleViaSearch",
                table: "Walls");
        }
    }
}
