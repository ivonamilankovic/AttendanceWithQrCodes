using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceWithQrCodes.Migrations
{
    /// <inheritdoc />
    public partial class StudentAttendances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Present = table.Column<bool>(type: "bit", nullable: false),
                    StudentIndex = table.Column<int>(type: "int", nullable: true),
                    StudentInformationIndex = table.Column<int>(type: "int", nullable: true),
                    LectureId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAttendances_Lectures_LectureId",
                        column: x => x.LectureId,
                        principalTable: "Lectures",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentAttendances_StudentInformations_StudentInformationIndex",
                        column: x => x.StudentInformationIndex,
                        principalTable: "StudentInformations",
                        principalColumn: "Index");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_LectureId",
                table: "StudentAttendances",
                column: "LectureId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_StudentInformationIndex",
                table: "StudentAttendances",
                column: "StudentInformationIndex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentAttendances");
        }
    }
}
