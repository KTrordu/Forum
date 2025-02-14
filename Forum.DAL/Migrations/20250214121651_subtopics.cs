using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forum.DAL.Migrations
{
    /// <inheritdoc />
    public partial class subtopics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Topics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "Topics",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_TopicId",
                table: "Topics",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Topics_TopicId",
                table: "Topics",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Topics_TopicId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_TopicId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Topics");
        }
    }
}
