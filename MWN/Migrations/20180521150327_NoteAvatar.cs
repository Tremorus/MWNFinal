using Microsoft.EntityFrameworkCore.Migrations;

namespace MWN.Migrations
{
    public partial class NoteAvatar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NoteAvatar",
                table: "Note",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteAvatar",
                table: "Note");
        }
    }
}
