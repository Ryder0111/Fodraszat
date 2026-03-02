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
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    { 1, "admin", "admin", "100000.mEabUMHOzNcI9WB/KT4VdA==.FfYF1tCJo/1N3JhTsxMgoy9aMa4JnMizC/2V7kDek2E=", 4, 0 },
                    { 2, "anna.kovacs@gmail.com", "Anna Kovács", "100000.yuczWUwlLNtGQPeeRALcSw==.69camw4RQIwaFLC0Nja0rr/CvHFZMwTFuFX7kE2sT4M=", 1, 2 },
                    { 3, "peter.nagy@gmail.com", "Péter Nagy", "100000.cmaT/7+hSczl/Vljd1RhpA==.yRICHU0JRKIfFEnnnOXqCclxaXJHSAyhC0gLGOaDWK8=", 1, 1 },
                    { 4, "eszter.fodrasz@gmail.com", "Eszter Fodrász", "100000.u0cbJvNnlmVBSs4kqmduAg==./ZEYmSE1468c2pBJp27DcH6XD3Nh2/eov2dIUf5QDaE=", 2, 2 },
                    { 5, "gabor.fodrasz@gmail.com", "Gábor Fodrász", "100000.F3hEuPibdr8ARoeuVj1V+A==.MZ6TIHP0hV6O6wxzmL1jRpsm4fypz8WUhb8sChK78LY=", 2, 1 }
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
