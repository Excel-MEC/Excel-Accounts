using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class RemovedDefaultInstitutionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InstitutionId",
                table: "Users",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldDefaultValue: 456);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InstitutionId",
                table: "Users",
                type: "integer",
                nullable: true,
                defaultValue: 456,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
