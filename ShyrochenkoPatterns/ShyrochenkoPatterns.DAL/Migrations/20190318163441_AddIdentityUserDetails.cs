using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShyrochenkoPatterns.DAL.Migrations
{
    public partial class AddIdentityUserDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SMSLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Sender = table.Column<string>(maxLength: 15, nullable: true),
                    Recipient = table.Column<string>(maxLength: 15, nullable: true),
                    Text = table.Column<string>(maxLength: 200, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMSLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerificationTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    TokenHash = table.Column<string>(maxLength: 200, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTokens_UserId",
                table: "VerificationTokens",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SMSLogs");

            migrationBuilder.DropTable(
                name: "VerificationTokens");
        }
    }
}
