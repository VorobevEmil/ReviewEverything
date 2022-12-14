using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewEverything.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateUpdateReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdationDate",
                table: "Reviews",
                newName: "UpdateDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Reviews",
                newName: "UpdationDate");
        }
    }
}
