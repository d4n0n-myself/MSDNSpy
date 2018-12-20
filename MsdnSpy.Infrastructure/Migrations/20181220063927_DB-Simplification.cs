using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MsdnSpy.Infrastructure.Migrations
{
    public partial class DBSimplification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Preferences",
                table: "UserPreferences");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddColumn<string>(
                name: "Preferences",
                table: "Users",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "UserPreferences",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Preferences",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Users",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserPreferences",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Preferences",
                table: "UserPreferences",
                nullable: true);
        }
    }
}
