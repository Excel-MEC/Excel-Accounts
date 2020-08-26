using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class AddCategoryIdToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "InstitutionId",
                table: "Users",
                nullable: true,
                defaultValue: 456,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "InstitutionId",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 456);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}
