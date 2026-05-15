using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelOwnershipAndSubmissionTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUserSubmitted",
                table: "hotels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OwnerUserId",
                table: "hotels",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionType",
                table: "hotel_submissions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetHotelId",
                table: "hotel_submissions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_hotels_OwnerUserId",
                table: "hotels",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_submissions_SubmissionType",
                table: "hotel_submissions",
                column: "SubmissionType");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_submissions_TargetHotelId",
                table: "hotel_submissions",
                column: "TargetHotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_hotels_OwnerUserId",
                table: "hotels");

            migrationBuilder.DropIndex(
                name: "IX_hotel_submissions_SubmissionType",
                table: "hotel_submissions");

            migrationBuilder.DropIndex(
                name: "IX_hotel_submissions_TargetHotelId",
                table: "hotel_submissions");

            migrationBuilder.DropColumn(
                name: "IsUserSubmitted",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "hotels");

            migrationBuilder.DropColumn(
                name: "SubmissionType",
                table: "hotel_submissions");

            migrationBuilder.DropColumn(
                name: "TargetHotelId",
                table: "hotel_submissions");
        }
    }
}
