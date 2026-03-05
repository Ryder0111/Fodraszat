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
                    AppointmentStatus = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    { 1, "admin", "admin", "100000.xJee9rsJ3BCY6B67PjDVAg==.pOLyoXHmCIN1zXEgoZ5gH3T05SfsXE1Y1DW+l14SJPo=", 4, 0 },
                    { 2, "anna.kovacs@gmail.com", "Anna Kovács", "100000.Yli5RTvANw7/z1l80OGgWg==.fW6ppLLGuX8tZZ8/B5949wT5xBZ6QKGjN4+7thH2Rbk=", 1, 2 },
                    { 3, "peter.nagy@gmail.com", "Péter Nagy", "100000.vdBl0bsVopLQKNX3Wlj/sg==.obDhu6r2uVmuag8gBVVvYJyDEU85gWeoctYFokSZVuc=", 1, 1 },
                    { 4, "gabor.fodrasz@gmail.com", "Nagy Gábor", "100000.n8xtN489r42l7WtfMmmmfw==.7Fy7c9tYeVJAHtf9NfePLhZ6L4yV+3BSyOnkwRJ+fsk=", 2, 2 },
                    { 5, "marcell.fodrasz@gmail.com", "Belák Marcell", "100000.MpbNpQCRUnK3K9DA+OhEhw==.ZQSk5zQtP4t262wwW6Ed8VGqoJaToxZNXRAhFBuJXVI=", 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "AppointmentId", "AppointmentStatus", "EndTime", "HairdresserId", "Notes", "ServiceId", "StartTime", "UserId" },
                values: new object[,]
                {
                    { 1, 0, new DateTime(2026, 3, 5, 10, 45, 0, 0, DateTimeKind.Unspecified), 4, "Férfi hajvágás", 2, new DateTime(2026, 3, 5, 10, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 2, 0, new DateTime(2026, 3, 5, 11, 45, 0, 0, DateTimeKind.Unspecified), 4, "Női hajvágás", 1, new DateTime(2026, 3, 5, 10, 45, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 3, 0, new DateTime(2026, 3, 5, 13, 45, 0, 0, DateTimeKind.Unspecified), 4, "Hajfestés", 3, new DateTime(2026, 3, 5, 11, 45, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, 0, new DateTime(2026, 3, 5, 15, 15, 0, 0, DateTimeKind.Unspecified), 4, "Melírozás", 4, new DateTime(2026, 3, 5, 13, 45, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 5, 0, new DateTime(2026, 3, 5, 16, 15, 0, 0, DateTimeKind.Unspecified), 4, "Frizura készítés", 5, new DateTime(2026, 3, 5, 15, 15, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 6, 0, new DateTime(2026, 3, 5, 17, 0, 0, 0, DateTimeKind.Unspecified), 4, "Férfi hajvágás", 2, new DateTime(2026, 3, 5, 16, 15, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 7, 0, new DateTime(2026, 3, 5, 18, 0, 0, 0, DateTimeKind.Unspecified), 4, "Női hajvágás", 1, new DateTime(2026, 3, 5, 17, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 8, 0, new DateTime(2026, 3, 10, 16, 0, 0, 0, DateTimeKind.Unspecified), 5, "Hajfestés délután", 3, new DateTime(2026, 3, 10, 14, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 9, 0, new DateTime(2026, 3, 10, 17, 30, 0, 0, DateTimeKind.Unspecified), 5, "Melírozás", 4, new DateTime(2026, 3, 10, 16, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 10, 0, new DateTime(2026, 3, 20, 12, 0, 0, 0, DateTimeKind.Unspecified), 4, "Reggeli hajfestés", 3, new DateTime(2026, 3, 20, 10, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 11, 0, new DateTime(2026, 3, 20, 14, 0, 0, 0, DateTimeKind.Unspecified), 4, "Délutáni női hajvágás", 1, new DateTime(2026, 3, 20, 13, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 12, 0, new DateTime(2026, 3, 15, 11, 0, 0, 0, DateTimeKind.Unspecified), 5, "Frizura reggel", 5, new DateTime(2026, 3, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 13, 0, new DateTime(2026, 3, 15, 14, 45, 0, 0, DateTimeKind.Unspecified), 5, "Férfi hajvágás délután", 2, new DateTime(2026, 3, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), 2 }
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
