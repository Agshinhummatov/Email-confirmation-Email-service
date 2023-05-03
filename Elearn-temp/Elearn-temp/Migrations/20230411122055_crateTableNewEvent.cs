using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elearn_temp.Migrations
{
    public partial class crateTableNewEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Evet",
                table: "Evet");

            migrationBuilder.RenameTable(
                name: "Evet",
                newName: "Events");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Evet");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Evet",
                table: "Evet",
                column: "Id");
        }
    }
}
