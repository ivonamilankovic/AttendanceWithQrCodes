using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceWithQrCodes.Migrations
{
    /// <inheritdoc />
    public partial class CoursesStudyProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoursesStudyProfiles",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    StudyProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_CoursesStudyProfiles_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CoursesStudyProfiles_StudyProfiles_StudyProfileId",
                        column: x => x.StudyProfileId,
                        principalTable: "StudyProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursesStudyProfiles_CourseId",
                table: "CoursesStudyProfiles",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesStudyProfiles_StudyProfileId",
                table: "CoursesStudyProfiles",
                column: "StudyProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursesStudyProfiles");
        }
    }
}
