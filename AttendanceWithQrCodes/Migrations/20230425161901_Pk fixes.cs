using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceWithQrCodes.Migrations
{
    /// <inheritdoc />
    public partial class Pkfixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CoursesStudyProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CoursesLanguages",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesStudyProfiles",
                table: "CoursesStudyProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursesLanguages",
                table: "CoursesLanguages",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesStudyProfiles",
                table: "CoursesStudyProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursesLanguages",
                table: "CoursesLanguages");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CoursesStudyProfiles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CoursesLanguages");
        }
    }
}
