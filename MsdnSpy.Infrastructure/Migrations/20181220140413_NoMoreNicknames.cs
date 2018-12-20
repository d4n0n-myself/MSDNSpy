using Microsoft.EntityFrameworkCore.Migrations;

namespace MsdnSpy.Infrastructure.Migrations
{
    public partial class NoMoreNicknames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                nullable: true);
        }
    }
}
