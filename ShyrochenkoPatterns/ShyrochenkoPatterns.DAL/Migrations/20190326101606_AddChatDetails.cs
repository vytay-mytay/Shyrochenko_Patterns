using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShyrochenkoPatterns.DAL.Migrations
{
    public partial class AddChatDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_LastItems_LastItemId",
                table: "Chats");

            migrationBuilder.DropTable(
                name: "LastItems");

            migrationBuilder.DropIndex(
                name: "IX_Chats_LastItemId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "LastReaded",
                table: "ChatUsers",
                newName: "LastReadMessageId");

            migrationBuilder.RenameColumn(
                name: "LastItemId",
                table: "Chats",
                newName: "LastMessageId");

            migrationBuilder.AddColumn<int>(
                name: "MessageStatus",
                table: "Messages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats",
                column: "LastMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Messages_LastMessageId",
                table: "Chats",
                column: "LastMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Messages_LastMessageId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "MessageStatus",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "LastReadMessageId",
                table: "ChatUsers",
                newName: "LastReaded");

            migrationBuilder.RenameColumn(
                name: "LastMessageId",
                table: "Chats",
                newName: "LastItemId");

            migrationBuilder.CreateTable(
                name: "LastItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChatId = table.Column<int>(nullable: true),
                    MessageId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LastItems_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LastItems_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastItemId",
                table: "Chats",
                column: "LastItemId",
                unique: true,
                filter: "[LastItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LastItems_MessageId",
                table: "LastItems",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_LastItems_UserId",
                table: "LastItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_LastItems_LastItemId",
                table: "Chats",
                column: "LastItemId",
                principalTable: "LastItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
