using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FodraszatIdopont.Migrations
{
    /// <inheritdoc />
    public partial class InitilCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationInMinute = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HairdresserId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppointmentStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointments_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_HairdresserId",
                        column: x => x.HairdresserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "ServiceId", "DurationInMinute", "Name", "Price", "isActive" },
                values: new object[,]
                {
                    { 1, 60, "Női hajvágás", 6000, true },
                    { 2, 45, "Férfi hajvágás", 4000, true },
                    { 3, 120, "Hajfestés", 15000, true },
                    { 4, 90, "Melírozás", 12000, true },
                    { 5, 60, "Frizura készítés", 7000, true }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Name", "PasswordHash", "Role", "Sex" },
                values: new object[,]
                {
                    { 1, "admin", "admin", "100000.6dFbp8r5FRlLPbrmED2JWA==.hU9lcNtCig7U7oeusbXsfN32sDhXFIgVw+084P/NqCM=", 4, 0 },
                    { 2, "anna.kovacs@gmail.com", "Anna Kovács", "100000.DTcXGyTwPm9N5SdL5OLD7w==.ciWAFveZSuQGFpTU82gMInqo5T6jZZnDQaDSeEEKVak=", 1, 2 },
                    { 3, "peter.nagy@gmail.com", "Péter Nagy", "100000.7kgeQgQIQ4n4L0pYVLaD3Q==.pSaaNjPeNo0VZskW+EYcT0oPoAV3VWPTWHUGXOAI8oo=", 1, 1 },
                    { 4, "gabor.fodrasz@gmail.com", "Nagy Gábor", "100000.zcwmQBDiSa3x9JJ8McJ3ZQ==.uSp3XCg+H5zSWXU7pXShwCTlaVqWcjIwTgKD6iOg7mY=", 2, 2 },
                    { 5, "marcell.fodrasz@gmail.com", "Belák Marcell", "100000.3vpnyJ0gLS3Yx07cQ5v16w==.ICYDUCec3bDLjXqbnA5ptWJgm2JUtukT+XW34L4h/rw=", 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_HairdresserId",
                table: "Appointments",
                column: "HairdresserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceId",
                table: "Appointments",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserId",
                table: "Appointments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
