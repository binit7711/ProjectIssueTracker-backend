using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectIssueTracker.Migrations
{
    /// <inheritdoc />
    public partial class NEWMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Issues_IssueId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_CommenterId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCollaborator_Projects_ProjectId",
                table: "ProjectCollaborator");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCollaborator_Users_UserId",
                table: "ProjectCollaborator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectCollaborator",
                table: "ProjectCollaborator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "ProjectCollaborator",
                newName: "ProjectCollaborators");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectCollaborator_ProjectId",
                table: "ProjectCollaborators",
                newName: "IX_ProjectCollaborators_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_IssueId",
                table: "Comments",
                newName: "IX_Comments_IssueId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_CommenterId",
                table: "Comments",
                newName: "IX_Comments_CommenterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectCollaborators",
                table: "ProjectCollaborators",
                columns: new[] { "UserId", "ProjectId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Issues_IssueId",
                table: "Comments",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_CommenterId",
                table: "Comments",
                column: "CommenterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCollaborators_Projects_ProjectId",
                table: "ProjectCollaborators",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCollaborators_Users_UserId",
                table: "ProjectCollaborators",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Issues_IssueId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_CommenterId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCollaborators_Projects_ProjectId",
                table: "ProjectCollaborators");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectCollaborators_Users_UserId",
                table: "ProjectCollaborators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectCollaborators",
                table: "ProjectCollaborators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "ProjectCollaborators",
                newName: "ProjectCollaborator");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectCollaborators_ProjectId",
                table: "ProjectCollaborator",
                newName: "IX_ProjectCollaborator_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_IssueId",
                table: "Comment",
                newName: "IX_Comment_IssueId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_CommenterId",
                table: "Comment",
                newName: "IX_Comment_CommenterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectCollaborator",
                table: "ProjectCollaborator",
                columns: new[] { "UserId", "ProjectId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Issues_IssueId",
                table: "Comment",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_CommenterId",
                table: "Comment",
                column: "CommenterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCollaborator_Projects_ProjectId",
                table: "ProjectCollaborator",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectCollaborator_Users_UserId",
                table: "ProjectCollaborator",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
