using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FodraszatIdopont.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    { 1, "admin", "admin", "100000.N2u9Ytm/+h17JxxBZHgyKg==.45Qb88rM+mjEbCCwjyxXKyxtRExahnaLPO/QOtxfYi8=", 4, 0 },
                    { 2, "anna.kovacs@gmail.com", "Anna Kovács", "100000.clx32FeirbAjWwF9thxz6w==.vCMU7ZuAQIEzQtb4svDByiBfzAdRoq0E4EBSKhoRSBk=", 1, 2 },
                    { 3, "peter.nagy@gmail.com", "Péter Nagy", "100000.lg2qwi6lAll6TWCHQxh3aA==.6p8UAfn1l2wCWoTnMr9pTbjJLJRDoy2BLzh1ADGanEY=", 1, 1 },
                    { 4, "gabor.fodrasz@gmail.com", "Nagy Gábor", "100000.bTTyZUi0jj6aPQ2q53n7Mg==.702A+Wu9tnbpvC0I2WEvmzkxktMSunXWezOoqKQfLQs=", 2, 2 },
                    { 5, "marcell.fodrasz@gmail.com", "Belák Marcell", "100000.7/kIc+smoq/Q+kH6K2lcGw==.rDGaTVLARvmEX/HXqcCrFch1mfTUygUDYdZa4NGt8UE=", 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "AppointmentId", "AppointmentStatus", "EndTime", "HairdresserId", "ServiceId", "StartTime", "UserId" },
                values: new object[,]
                {
                    { 1, 0, new DateTime(2026, 4, 10, 9, 30, 0, 0, DateTimeKind.Unspecified), 2, 1, new DateTime(2026, 4, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 0, new DateTime(2026, 4, 10, 11, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, new DateTime(2026, 4, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, 2, new DateTime(2025, 12, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, new DateTime(2025, 12, 15, 8, 30, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 4, 1, new DateTime(2025, 11, 20, 10, 30, 0, 0, DateTimeKind.Unspecified), 3, 3, new DateTime(2025, 11, 20, 9, 30, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 5, 0, new DateTime(2026, 4, 12, 14, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, new DateTime(2026, 4, 12, 13, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 6, 2, new DateTime(2025, 10, 5, 15, 0, 0, 0, DateTimeKind.Unspecified), 3, 1, new DateTime(2025, 10, 5, 14, 30, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 7, 0, new DateTime(2026, 4, 13, 11, 30, 0, 0, DateTimeKind.Unspecified), 4, 3, new DateTime(2026, 4, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 8, 2, new DateTime(2025, 9, 18, 13, 0, 0, 0, DateTimeKind.Unspecified), 4, 2, new DateTime(2025, 9, 18, 12, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 9, 0, new DateTime(2026, 4, 14, 9, 30, 0, 0, DateTimeKind.Unspecified), 2, 1, new DateTime(2026, 4, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 10, 1, new DateTime(2025, 8, 22, 12, 30, 0, 0, DateTimeKind.Unspecified), 3, 3, new DateTime(2025, 8, 22, 11, 0, 0, 0, DateTimeKind.Unspecified), 5 }
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
