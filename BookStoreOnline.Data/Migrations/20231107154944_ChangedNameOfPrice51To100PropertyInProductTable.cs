using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreOnline.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNameOfPrice51To100PropertyInProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price50To100",
                table: "Products",
                newName: "Price51To100");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price51To100",
                table: "Products",
                newName: "Price50To100");
        }
    }
}
