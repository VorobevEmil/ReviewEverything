using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewEverything.Server.Migrations
{
    /// <inheritdoc />
    public partial class DeleteAlternariveKeyFromReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Reviews_CompositionId_AuthorId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CompositionId",
                table: "Reviews",
                column: "CompositionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_CompositionId",
                table: "Reviews");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Reviews_CompositionId_AuthorId",
                table: "Reviews",
                columns: new[] { "CompositionId", "AuthorId" });
        }
    }
}
