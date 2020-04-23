using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddedtheForeignKeyfromAbassadorTableandRemovedaForeignKeyinUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Ambassadors_AmbassadorId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AmbassadorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AmbassadorId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Ambassadors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ambassadors_UserId",
                table: "Ambassadors",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ambassadors_Users_UserId",
                table: "Ambassadors",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ambassadors_Users_UserId",
                table: "Ambassadors");

            migrationBuilder.DropIndex(
                name: "IX_Ambassadors_UserId",
                table: "Ambassadors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Ambassadors");

            migrationBuilder.AddColumn<int>(
                name: "AmbassadorId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AmbassadorId",
                table: "Users",
                column: "AmbassadorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Ambassadors_AmbassadorId",
                table: "Users",
                column: "AmbassadorId",
                principalTable: "Ambassadors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
