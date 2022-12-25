using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewEverything.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedBlockPropertyInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Block",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Block",
                table: "AspNetUsers");
        }
    }
}
