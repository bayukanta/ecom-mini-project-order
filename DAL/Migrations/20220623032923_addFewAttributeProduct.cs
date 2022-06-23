using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addFewAttributeProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ProductDetails",
                newName: "HargaJual");

            migrationBuilder.AddColumn<int>(
                name: "HargaAwal",
                table: "ProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "ProductDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Keterangan",
                table: "ProductDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HargaAwal",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Img",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Keterangan",
                table: "ProductDetails");

            migrationBuilder.RenameColumn(
                name: "HargaJual",
                table: "ProductDetails",
                newName: "Price");
        }
    }
}
