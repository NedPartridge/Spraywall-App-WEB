using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpraywallAppWeb.Migrations
{
    /// <inheritdoc />
    public partial class ClimbRelations1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Climb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SetterName = table.Column<string>(type: "TEXT", nullable: false),
                    Grade = table.Column<int>(type: "INTEGER", nullable: false),
                    JsonHolds = table.Column<string>(type: "TEXT", nullable: false),
                    WallID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Climb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Climb_Walls_WallID",
                        column: x => x.WallID,
                        principalTable: "Walls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClimb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClimbId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfAttempts = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClimb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClimb_Climb_ClimbId",
                        column: x => x.ClimbId,
                        principalTable: "Climb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserClimb_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Climb_WallID",
                table: "Climb",
                column: "WallID");

            migrationBuilder.CreateIndex(
                name: "IX_UserClimb_ClimbId",
                table: "UserClimb",
                column: "ClimbId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClimb_UserId",
                table: "UserClimb",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClimb");

            migrationBuilder.DropTable(
                name: "Climb");
        }
    }
}
