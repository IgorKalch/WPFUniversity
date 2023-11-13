using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class InsertData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Name", "Description" },
                values: new object[,]
                {
            { "History", "World history and civilizations" },
            { "Literature", "Classic and modern literature" },
            { "Mathematics", "Algebra and calculus" },
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "CourseId", "FirstName", "LastName", "Subject" },
                values: new object[,]
                {
            { 1, "John", "Smith", "History" },
            { 2, "Emily", "Johnson", "Literature" },
            { 3, "Michael", "Brown", "Math" },
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "CourseId", "TeacherId", "Name" },
                values: new object[,]
                {
            { 1, 1, "History Group 1" },
            { 2, 2, "Literature Group 1" },
            { 3, 3, "Math Group 1" },
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "GroupId", "FirstName", "LastName" },
                values: new object[,]
                {
            { 1, "Michael", "Brown" },
            { 2, "Olivia", "Taylor" },
            { 3, "Ethan", "Williams" },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                 name: "Students");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
