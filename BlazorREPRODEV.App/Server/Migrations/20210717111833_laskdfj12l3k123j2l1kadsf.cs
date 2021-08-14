using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorREPRODEV.App.Server.Migrations
{
    public partial class laskdfj12l3k123j2l1kadsf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IsRemoved",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemoved",
                table: "Materials");
        }
    }
}
