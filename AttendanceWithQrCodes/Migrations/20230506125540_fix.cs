using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceWithQrCodes.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendances_StudentInformations_StudentInformationIndex",
                table: "StudentAttendances");

            migrationBuilder.DropIndex(
                name: "IX_StudentAttendances_StudentInformationIndex",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "StudentInformationIndex",
                table: "StudentAttendances");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_StudentIndex",
                table: "StudentAttendances",
                column: "StudentIndex");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendances_StudentInformations_StudentIndex",
                table: "StudentAttendances",
                column: "StudentIndex",
                principalTable: "StudentInformations",
                principalColumn: "Index");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendances_StudentInformations_StudentIndex",
                table: "StudentAttendances");

            migrationBuilder.DropIndex(
                name: "IX_StudentAttendances_StudentIndex",
                table: "StudentAttendances");

            migrationBuilder.AddColumn<int>(
                name: "StudentInformationIndex",
                table: "StudentAttendances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_StudentInformationIndex",
                table: "StudentAttendances",
                column: "StudentInformationIndex");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendances_StudentInformations_StudentInformationIndex",
                table: "StudentAttendances",
                column: "StudentInformationIndex",
                principalTable: "StudentInformations",
                principalColumn: "Index");
        }
    }
}
