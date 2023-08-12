using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Migrations
{
    /// <inheritdoc />
    public partial class Update_Customers_TB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "Mobile",
                table: "Customers",
                newName: "SecondPostalCode");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Customers",
                newName: "SecondAddress");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Customers",
                newName: "FirstPostalCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecondPostalCode",
                table: "Customers",
                newName: "Mobile");

            migrationBuilder.RenameColumn(
                name: "SecondAddress",
                table: "Customers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "FirstPostalCode",
                table: "Customers",
                newName: "FirstName");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
