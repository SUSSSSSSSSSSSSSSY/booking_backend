using System;
using System.Collections.Generic;
using Booking.Domain.Hotels;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hotel_submissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SubmittedByUserId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ApprovedHotelId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PricePerNight = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DistanceToCenterKm = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Tags = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Amenities = table.Column<List<string>>(type: "jsonb", nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Images = table.Column<List<string>>(type: "jsonb", nullable: false),
                    ScoreItems = table.Column<List<ScoreItem>>(type: "jsonb", nullable: false),
                    Facilities = table.Column<List<FacilityGroup>>(type: "jsonb", nullable: false),
                    Rooms = table.Column<List<Room>>(type: "jsonb", nullable: false),
                    AdminComment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedByAdminId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_hotel_submissions_users_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_hotel_submissions_users_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hotel_submissions_CreatedAtUtc",
                table: "hotel_submissions",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_submissions_ReviewedByAdminId",
                table: "hotel_submissions",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_submissions_Status",
                table: "hotel_submissions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_submissions_SubmittedByUserId",
                table: "hotel_submissions",
                column: "SubmittedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hotel_submissions");
        }
    }
}
