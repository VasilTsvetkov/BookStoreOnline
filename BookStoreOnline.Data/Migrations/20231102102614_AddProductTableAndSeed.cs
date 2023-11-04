using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStoreOnline.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTableAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price50To100 = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceOver100 = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price50To100", "PriceOver100", "Title" },
                values: new object[,]
                {
                    { 1, "F. Scott Fitzgerald", "A classic novel about the American Dream in the Jazz Age.", "978-0743273565", 19.99m, 15.99m, 14.99m, 13.99m, "The Great Gatsby" },
                    { 2, "Harper Lee", "A powerful story about racial injustice in the American South.", "978-0061120084", 18.99m, 16.99m, 15.99m, 14.99m, "To Kill a Mockingbird" },
                    { 3, "George Orwell", "A dystopian novel that explores the dangers of totalitarianism.", "978-0451524935", 16.99m, 13.99m, 12.99m, 11.99m, "1984" },
                    { 4, "J.D. Salinger", "A coming-of-age novel following the adventures of Holden Caulfield.", "978-0316769174", 15.99m, 12.99m, 11.99m, 10.99m, "The Catcher in the Rye" },
                    { 5, "Jane Austen", "A classic romance novel set in 19th-century England.", "978-0141439518", 17.99m, 14.99m, 13.99m, 12.99m, "Pride and Prejudice" },
                    { 6, "J.R.R. Tolkien", "A fantasy adventure about the journey of Bilbo Baggins.", "978-0345534835", 20.99m, 17.99m, 16.99m, 15.99m, "The Hobbit" },
                    { 7, "Paulo Coelho", "A philosophical novel about pursuing one's dreams.", "978-0062315007", 14.99m, 11.99m, 10.99m, 9.99m, "The Alchemist" },
                    { 8, "J.K. Rowling", "The first book in the popular Harry Potter series.", "978-0590353427", 21.99m, 18.99m, 17.99m, 16.99m, "Harry Potter and the Sorcerer's Stone" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
