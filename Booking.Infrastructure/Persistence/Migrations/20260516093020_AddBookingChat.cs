using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chat_threads",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BookingId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    GuestUserId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OwnerUserId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    HotelId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RoomId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastMessageAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_threads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ThreadId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SenderUserId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Text = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsReadByGuest = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadByOwner = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chat_messages_chat_threads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "chat_threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_CreatedAtUtc",
                table: "chat_messages",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_SenderUserId",
                table: "chat_messages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_ThreadId",
                table: "chat_messages",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_threads_BookingId",
                table: "chat_threads",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chat_threads_GuestUserId",
                table: "chat_threads",
                column: "GuestUserId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_threads_HotelId",
                table: "chat_threads",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_threads_LastMessageAtUtc",
                table: "chat_threads",
                column: "LastMessageAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_chat_threads_OwnerUserId",
                table: "chat_threads",
                column: "OwnerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_messages");

            migrationBuilder.DropTable(
                name: "chat_threads");
        }
    }
}
