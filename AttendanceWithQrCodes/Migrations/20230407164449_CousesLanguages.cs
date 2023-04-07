using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceWithQrCodes.Migrations
{
    /// <inheritdoc />
    public partial class CousesLanguages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_StudyLanguages_StudyLanguageId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_StudyLanguageId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StudyLanguageId",
                table: "Courses");

            migrationBuilder.CreateTable(
                name: "CoursesLanguages",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    StudyLanguageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_CoursesLanguages_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CoursesLanguages_StudyLanguages_StudyLanguageId",
                        column: x => x.StudyLanguageId,
                        principalTable: "StudyLanguages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursesLanguages_CourseId",
                table: "CoursesLanguages",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesLanguages_StudyLanguageId",
                table: "CoursesLanguages",
                column: "StudyLanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursesLanguages");

            migrationBuilder.AddColumn<int>(
                name: "StudyLanguageId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_StudyLanguageId",
                table: "Courses",
                column: "StudyLanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_StudyLanguages_StudyLanguageId",
                table: "Courses",
                column: "StudyLanguageId",
                principalTable: "StudyLanguages",
                principalColumn: "Id");
        }
    }
}
