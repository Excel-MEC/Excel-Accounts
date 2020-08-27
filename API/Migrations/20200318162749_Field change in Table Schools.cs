using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Migrations
{
    public partial class FieldchangeinTableSchools : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "Schools");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Schools",
                type: "text",
                nullable: true);
        }
    }
}
