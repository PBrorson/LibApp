using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryApplication.Database.Migrations
{
    /// <inheritdoc />
    public partial class TitleAcronym : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TitleAcronym",
                table: "LibraryItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleAcronym",
                table: "LibraryItems");
        }
    }
}
