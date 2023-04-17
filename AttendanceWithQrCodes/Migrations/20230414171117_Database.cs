using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceWithQrCodes.Migrations
{
    /// <inheritdoc />
    public partial class Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QrCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudyLanguages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyLanguages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudyProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

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
                    AssistantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "StudentInformations",
                columns: table => new
                {
                    Index = table.Column<int>(type: "int", nullable: false),
                    MacAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    StudyProfileId = table.Column<int>(type: "int", nullable: true),
                    StudyLanguageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentInformations", x => x.Index);
                    table.ForeignKey(
                        name: "FK_StudentInformations_StudyLanguages_StudyLanguageId",
                        column: x => x.StudyLanguageId,
                        principalTable: "StudyLanguages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentInformations_StudyProfiles_StudyProfileId",
                        column: x => x.StudyProfileId,
                        principalTable: "StudyProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentInformations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateTable(
                name: "Lectures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    LecturerId = table.Column<int>(type: "int", nullable: true),
                    QrCodeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lectures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lectures_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lectures_QrCodes_QrCodeId",
                        column: x => x.QrCodeId,
                        principalTable: "QrCodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lectures_Users_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

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
                name: "IX_Courses_AssistantId",
                table: "Courses",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ProfessorId",
                table: "Courses",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesLanguages_CourseId",
                table: "CoursesLanguages",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesLanguages_StudyLanguageId",
                table: "CoursesLanguages",
                column: "StudyLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesStudyProfiles_CourseId",
                table: "CoursesStudyProfiles",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesStudyProfiles_StudyProfileId",
                table: "CoursesStudyProfiles",
                column: "StudyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_CourseId",
                table: "Lectures",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_LecturerId",
                table: "Lectures",
                column: "LecturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_QrCodeId",
                table: "Lectures",
                column: "QrCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_LectureId",
                table: "StudentAttendances",
                column: "LectureId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAttendances_StudentInformationIndex",
                table: "StudentAttendances",
                column: "StudentInformationIndex");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInformations_StudyLanguageId",
                table: "StudentInformations",
                column: "StudyLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInformations_StudyProfileId",
                table: "StudentInformations",
                column: "StudyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInformations_UserId",
                table: "StudentInformations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursesLanguages");

            migrationBuilder.DropTable(
                name: "CoursesStudyProfiles");

            migrationBuilder.DropTable(
                name: "StudentAttendances");

            migrationBuilder.DropTable(
                name: "Lectures");

            migrationBuilder.DropTable(
                name: "StudentInformations");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "QrCodes");

            migrationBuilder.DropTable(
                name: "StudyLanguages");

            migrationBuilder.DropTable(
                name: "StudyProfiles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
