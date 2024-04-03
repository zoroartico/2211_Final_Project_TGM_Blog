using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _2211_Final_Project_TGM_Blog.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedChatRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverUserId",
                table: "ChatRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverUserId",
                table: "ChatRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
