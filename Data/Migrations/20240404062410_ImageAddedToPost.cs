using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2211_Final_Project_TGM_Blog.Data.Migrations
{
    /// <inheritdoc />
    public partial class ImageAddedToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Posts",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Posts");
        }
    }
}
