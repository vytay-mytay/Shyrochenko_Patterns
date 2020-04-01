using Microsoft.EntityFrameworkCore.Migrations;

namespace ShyrochenkoPatterns.DAL.Migrations
{
    public partial class Create_EmailRecipients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recipient",
                table: "EmailLogs");

            migrationBuilder.CreateTable(
                name: "EmailRecipients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(maxLength: 129, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailRecipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailRecipients_EmailLogs_LogId",
                        column: x => x.LogId,
                        principalTable: "EmailLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecipients_LogId",
                table: "EmailRecipients",
                column: "LogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailRecipients");

            migrationBuilder.AddColumn<string>(
                name: "Recipient",
                table: "EmailLogs",
                maxLength: 129,
                nullable: true);
        }
    }
}
