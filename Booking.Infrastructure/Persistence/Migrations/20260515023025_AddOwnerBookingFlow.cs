using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerBookingFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "bookings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAtUtc",
                table: "bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HotelOwnerUserId",
                table: "bookings",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHiddenForUser",
                table: "bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "OwnerRespondedAtUtc",
                table: "bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_bookings_HotelOwnerUserId",
                table: "bookings",
                column: "HotelOwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_RoomId_CheckIn_CheckOut",
                table: "bookings",
                columns: new[] { "RoomId", "CheckIn", "CheckOut" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_bookings_HotelOwnerUserId",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_bookings_RoomId_CheckIn_CheckOut",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "CancelledAtUtc",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "HotelOwnerUserId",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "IsHiddenForUser",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "OwnerRespondedAtUtc",
                table: "bookings");
        }
    }
}
