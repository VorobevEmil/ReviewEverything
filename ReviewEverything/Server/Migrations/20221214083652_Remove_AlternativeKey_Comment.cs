using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewEverything.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAlternativeKeyComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Comments_ReviewId_UserId",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReviewId",
                table: "Comments",
                column: "ReviewId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_ReviewId",
                table: "Comments");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Comments_ReviewId_UserId",
                table: "Comments",
                columns: new[] { "ReviewId", "UserId" });
        }
    }
}
