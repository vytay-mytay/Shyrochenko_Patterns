using Microsoft.EntityFrameworkCore.Migrations;

namespace ShyrochenkoPatterns.DAL.Migrations
{
    public partial class FacebookAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationTokens_AspNetUsers_UserId",
                table: "VerificationTokens");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "VerificationTokens",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "VerificationTokens",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "VerificationTokens",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FacebookId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationTokens_AspNetUsers_UserId",
                table: "VerificationTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationTokens_AspNetUsers_UserId",
                table: "VerificationTokens");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "VerificationTokens");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "VerificationTokens");

            migrationBuilder.DropColumn(
                name: "FacebookId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "VerificationTokens",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationTokens_AspNetUsers_UserId",
                table: "VerificationTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
