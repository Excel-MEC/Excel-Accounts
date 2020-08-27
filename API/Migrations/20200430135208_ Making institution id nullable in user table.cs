using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class Makinginstitutionidnullableinusertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InstitutionId",
                table: "Users",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InstitutionId",
                table: "Users",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
