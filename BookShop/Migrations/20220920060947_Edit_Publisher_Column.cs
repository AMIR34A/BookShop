using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Migrations
{
    public partial class Edit_Publisher_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OublisherName",
                table: "Publishers",
                newName: "PublisherName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublisherName",
                table: "Publishers",
                newName: "OublisherName");
        }
    }
}
