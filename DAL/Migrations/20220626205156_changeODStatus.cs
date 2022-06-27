using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class changeODStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "ODStatus",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ODStatus",
                newName: "ODId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ODStatus",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "ODId",
                table: "ODStatus",
                newName: "Id");
        }
    }
}
