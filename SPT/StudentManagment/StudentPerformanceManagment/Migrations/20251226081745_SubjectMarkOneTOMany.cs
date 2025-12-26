using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPerformanceManagment.Migrations
{
    /// <inheritdoc />
    public partial class SubjectMarkOneTOMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Marks_SubjectId",
                table: "Marks");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_SubjectId",
                table: "Marks",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_SubjectId_StudentId",
                table: "Marks",
                columns: new[] { "SubjectId", "StudentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marks_Tasks_TasksId",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Marks_SubjectId",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Marks_SubjectId_StudentId",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Marks_TasksId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "TasksId",
                table: "Marks");

            migrationBuilder.CreateIndex(
                name: "IX_Marks_SubjectId",
                table: "Marks",
                column: "SubjectId",
                unique: true);
        }
    }
}
