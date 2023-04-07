using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceWithQrCodes.Migrations
{
    /// <inheritdoc />
    public partial class Courses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LecturesNumForProfessor = table.Column<int>(type: "int", nullable: false),
                    LecturesNumForAssistent = table.Column<int>(type: "int", nullable: true),
                    TotalTakenLectures = table.Column<int>(type: "int", nullable: false),
                    ProfessorId = table.Column<int>(type: "int", nullable: true),
                    AssistantId = table.Column<int>(type: "int", nullable: true),
                    StudyLanguageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_StudyLanguages_StudyLanguageId",
                        column: x => x.StudyLanguageId,
                        principalTable: "StudyLanguages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courses_Users_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courses_Users_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_AssistantId",
                table: "Courses",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ProfessorId",
                table: "Courses",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_StudyLanguageId",
                table: "Courses",
                column: "StudyLanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
