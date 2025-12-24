using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPerformanceManagment.Migrations
{
    /// <inheritdoc />
    public partial class MarkTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TasksId",
                table: "Marks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Marks_TasksId",
                table: "Marks",
                column: "TasksId");

            migrationBuilder.AddForeignKey(
                name: "FK_Marks_Tasks_TasksId",
                table: "Marks",
                column: "TasksId",
                principalTable: "Tasks",
                principalColumn: "TasksId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Marks_Tasks_TasksId",
                table: "Marks");

            migrationBuilder.DropIndex(
                name: "IX_Marks_TasksId",
                table: "Marks");

            migrationBuilder.DropColumn(
                name: "TasksId",
                table: "Marks");
        }
    }
}
