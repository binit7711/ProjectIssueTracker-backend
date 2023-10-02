using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectIssueTracker.Migrations
{
    /// <inheritdoc />
    public partial class FixesIdIssues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_CommenterUserId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Issues");

            migrationBuilder.RenameColumn(
                name: "CommenterUserId",
                table: "Comment",
                newName: "CommenterId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_CommenterUserId",
                table: "Comment",
                newName: "IX_Comment_CommenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_CommenterId",
                table: "Comment",
                column: "CommenterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_CommenterId",
                table: "Comment");

            migrationBuilder.RenameColumn(
                name: "CommenterId",
                table: "Comment",
                newName: "CommenterUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_CommenterId",
                table: "Comment",
                newName: "IX_Comment_CommenterUserId");

            migrationBuilder.AddColumn<int>(
                name: "CreatorUserId",
                table: "Issues",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_CommenterUserId",
                table: "Comment",
                column: "CommenterUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
