using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpraywallAppWeb.Migrations
{
    /// <inheritdoc />
    public partial class ClimbRelations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Climb_Walls_WallID",
                table: "Climb");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClimb_Climb_ClimbId",
                table: "UserClimb");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClimb_User_UserId",
                table: "UserClimb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClimb",
                table: "UserClimb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Climb",
                table: "Climb");

            migrationBuilder.RenameTable(
                name: "UserClimb",
                newName: "UserClimbs");

            migrationBuilder.RenameTable(
                name: "Climb",
                newName: "Climbs");

            migrationBuilder.RenameIndex(
                name: "IX_UserClimb_UserId",
                table: "UserClimbs",
                newName: "IX_UserClimbs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserClimb_ClimbId",
                table: "UserClimbs",
                newName: "IX_UserClimbs_ClimbId");

            migrationBuilder.RenameIndex(
                name: "IX_Climb_WallID",
                table: "Climbs",
                newName: "IX_Climbs_WallID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClimbs",
                table: "UserClimbs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Climbs",
                table: "Climbs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Climbs_Walls_WallID",
                table: "Climbs",
                column: "WallID",
                principalTable: "Walls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClimbs_Climbs_ClimbId",
                table: "UserClimbs",
                column: "ClimbId",
                principalTable: "Climbs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClimbs_User_UserId",
                table: "UserClimbs",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Climbs_Walls_WallID",
                table: "Climbs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClimbs_Climbs_ClimbId",
                table: "UserClimbs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClimbs_User_UserId",
                table: "UserClimbs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClimbs",
                table: "UserClimbs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Climbs",
                table: "Climbs");

            migrationBuilder.RenameTable(
                name: "UserClimbs",
                newName: "UserClimb");

            migrationBuilder.RenameTable(
                name: "Climbs",
                newName: "Climb");

            migrationBuilder.RenameIndex(
                name: "IX_UserClimbs_UserId",
                table: "UserClimb",
                newName: "IX_UserClimb_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserClimbs_ClimbId",
                table: "UserClimb",
                newName: "IX_UserClimb_ClimbId");

            migrationBuilder.RenameIndex(
                name: "IX_Climbs_WallID",
                table: "Climb",
                newName: "IX_Climb_WallID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClimb",
                table: "UserClimb",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Climb",
                table: "Climb",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Climb_Walls_WallID",
                table: "Climb",
                column: "WallID",
                principalTable: "Walls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClimb_Climb_ClimbId",
                table: "UserClimb",
                column: "ClimbId",
                principalTable: "Climb",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClimb_User_UserId",
                table: "UserClimb",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
