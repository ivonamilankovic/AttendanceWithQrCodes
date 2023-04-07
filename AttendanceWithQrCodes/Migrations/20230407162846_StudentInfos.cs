using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceWithQrCodes.Migrations
{
    /// <inheritdoc />
    public partial class StudentInfos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentInformations",
                columns: table => new
                {
                    Index = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentInformations");
        }
    }
}
