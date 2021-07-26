using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkoutApp.Migrations
{
    public partial class AddFieldToPostEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkoutId",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_WorkoutId",
                table: "Posts",
                column: "WorkoutId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Workouts_WorkoutId",
                table: "Posts",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Workouts_WorkoutId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_WorkoutId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "WorkoutId",
                table: "Posts");
        }
    }
}
