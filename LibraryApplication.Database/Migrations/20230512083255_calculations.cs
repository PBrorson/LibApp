using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryApplication.Database.Migrations
{
    /// <inheritdoc />
    public partial class calculations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FinePerDay",
                table: "LibraryItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinePerDay",
                table: "LibraryItems");
        }
    }
}
