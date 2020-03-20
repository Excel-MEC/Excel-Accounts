using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class UserSchemamodification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QRCodeUrl",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodeUrl",
                table: "Users");
        }
    }
}
