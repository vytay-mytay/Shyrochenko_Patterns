using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShyrochenkoPatterns.DAL.Migrations
{
    public partial class Create_Post_Proverb_Story : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisposedAt",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "ExpiresDate",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserTokens");

            migrationBuilder.AddColumn<DateTime>(
                name: "AccessExpiresDate",
                table: "UserTokens",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AccessTokenHash",
                table: "UserTokens",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshExpiresDate",
                table: "UserTokens",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RefreshTokenHash",
                table: "UserTokens",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Poems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Synopsis = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proverbs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proverbs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeriesId = table.Column<int>(nullable: false),
                    PartNumber = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Poems");

            migrationBuilder.DropTable(
                name: "Proverbs");

            migrationBuilder.DropTable(
                name: "Stories");

            migrationBuilder.DropColumn(
                name: "AccessExpiresDate",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "AccessTokenHash",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "RefreshExpiresDate",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "RefreshTokenHash",
                table: "UserTokens");

            migrationBuilder.AddColumn<DateTime>(
                name: "DisposedAt",
                table: "UserTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresDate",
                table: "UserTokens",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TokenHash",
                table: "UserTokens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "UserTokens",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
