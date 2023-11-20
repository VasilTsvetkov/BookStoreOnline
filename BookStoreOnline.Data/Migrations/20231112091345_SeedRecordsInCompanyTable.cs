using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStoreOnline.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRecordsInCompanyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "City", "Country", "Name", "PhoneNumber", "PostalCode" },
                values: new object[,]
                {
                    { 1, "Street 123", "City 123", "Country 123", "Microsoft", "854124724", "7521" },
                    { 2, "Street 456", "City 456", "Country 456", "Google", "875217896", "3587" },
                    { 3, "Street 789", "City 789", "Country 789", "Amazon", "854124724", "7851" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
