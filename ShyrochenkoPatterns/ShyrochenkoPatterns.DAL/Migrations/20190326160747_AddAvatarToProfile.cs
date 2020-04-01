using Microsoft.EntityFrameworkCore.Migrations;

namespace ShyrochenkoPatterns.DAL.Migrations
{
    public partial class AddAvatarToProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarSrc",
                table: "Profile");

            migrationBuilder.AddColumn<int>(
                name: "AvatarId",
                table: "Profile",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profile_AvatarId",
                table: "Profile",
                column: "AvatarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profile_Images_AvatarId",
                table: "Profile",
                column: "AvatarId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profile_Images_AvatarId",
                table: "Profile");

            migrationBuilder.DropIndex(
                name: "IX_Profile_AvatarId",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "Profile");

            migrationBuilder.AddColumn<string>(
                name: "AvatarSrc",
                table: "Profile",
                maxLength: 100,
                nullable: true);
        }
    }
}
