using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewEverything.Server.Migrations
{
    /// <inheritdoc />
    public partial class EditLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Comments_LikeCommentsId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "LikeCommentsId",
                table: "Likes",
                newName: "LikeReviewsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Reviews_LikeReviewsId",
                table: "Likes",
                column: "LikeReviewsId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Reviews_LikeReviewsId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "LikeReviewsId",
                table: "Likes",
                newName: "LikeCommentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Comments_LikeCommentsId",
                table: "Likes",
                column: "LikeCommentsId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
