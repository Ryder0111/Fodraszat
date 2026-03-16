using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FodraszatIdopont.Migrations
{
    /// <inheritdoc />
    public partial class ClosedDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClosedDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosedDays", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "100000.G5gQ0f7G0Fh3+1Kf0hkLjw==.5qypMaVTdhsZZYKAb7eE5LBfl65avM9Hd/QGLnREZIs=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "100000.7tZ2JiOK/sadUGITAQsL3Q==.dFpjOFCDjJ6EsBL4NCVs4ei8FmVnZkubH5NmFQnMINY=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "100000.kVOj5yPdgSGPkPaDEFQXJQ==.DITK5u53+F2Ki5TW5gzMSdbpuGlr0h7UPDPChbmB5Xc=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "PasswordHash",
                value: "100000.W+9sttCKklLVjSIOvyAEoQ==.wnH8u14ixrhXm17GizwWChxZaSAsJDS1Htgmzz8jCBI=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "PasswordHash",
                value: "100000.WwlDY/pWqZDuFZPeQYERCQ==.ejDdWOE9lEUIpLf6+IusAcRCiqRxXy92/tbV5s/4Hss=");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClosedDays");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "PasswordHash",
                value: "100000.6dFbp8r5FRlLPbrmED2JWA==.hU9lcNtCig7U7oeusbXsfN32sDhXFIgVw+084P/NqCM=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "PasswordHash",
                value: "100000.DTcXGyTwPm9N5SdL5OLD7w==.ciWAFveZSuQGFpTU82gMInqo5T6jZZnDQaDSeEEKVak=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "PasswordHash",
                value: "100000.7kgeQgQIQ4n4L0pYVLaD3Q==.pSaaNjPeNo0VZskW+EYcT0oPoAV3VWPTWHUGXOAI8oo=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "PasswordHash",
                value: "100000.zcwmQBDiSa3x9JJ8McJ3ZQ==.uSp3XCg+H5zSWXU7pXShwCTlaVqWcjIwTgKD6iOg7mY=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "PasswordHash",
                value: "100000.3vpnyJ0gLS3Yx07cQ5v16w==.ICYDUCec3bDLjXqbnA5ptWJgm2JUtukT+XW34L4h/rw=");
        }
    }
}
