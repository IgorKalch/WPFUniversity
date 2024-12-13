using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityDataLayer.Migrations;

/// <inheritdoc />
public partial class InsertData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        for (int i = 1; i <= 100; i++)
        {
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Name", "Description" },
                values: new object[] { i, $"Course {i}", $"Description for Course {i}" }
            );
        }

        for (int i = 1; i <= 100; i++)
        {
            int courseId = (i % 100) + 1;

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "FirstName", "LastName", "Subject", "CourseId" },
                values: new object[] { i, $"TeacherFirstName{i}", $"TeacherLastName{i}", $"Subject {i}", courseId }
            );
        }

        for (int i = 1; i <= 100; i++)
        {
            int courseId = (i % 100) + 1;
            int teacherId = (i % 100) + 1;

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Name", "CourseId", "TeacherId" },
                values: new object[] { i, $"Group {i}", courseId, teacherId }
            );
        }

        int studentId = 1;
        for (int groupId = 1; groupId <= 100; groupId++)
        {
            for (int s = 1; s <= 10; s++)
            {
                migrationBuilder.InsertData(
                    table: "Students",
                    columns: new[] { "Id", "FirstName", "LastName", "GroupId" },
                    values: new object[] { studentId, $"StudentFirstName{studentId}", $"StudentLastName{studentId}", groupId }
                );
                studentId++;
            }
        }
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        var studentKeyValues = new object[1000, 1];
        for (int i = 0; i < 1000; i++)
        {
            studentKeyValues[i, 0] = i + 1;
        }
        migrationBuilder.DeleteData(
            table: "Students",
            keyColumns: new[] { "Id" },
            keyValues: studentKeyValues
        );

        var groupKeyValues = new object[100, 1];
        for (int i = 0; i < 100; i++)
        {
            groupKeyValues[i, 0] = i + 1;
        }
        migrationBuilder.DeleteData(
            table: "Groups",
            keyColumns: new[] { "Id" },
            keyValues: groupKeyValues
        );

        var teacherKeyValues = new object[100, 1];
        for (int i = 0; i < 100; i++)
        {
            teacherKeyValues[i, 0] = i + 1;
        }
        migrationBuilder.DeleteData(
            table: "Teachers",
            keyColumns: new[] { "Id" },
            keyValues: teacherKeyValues
        );

        var courseKeyValues = new object[100, 1];
        for (int i = 0; i < 100; i++)
        {
            courseKeyValues[i, 0] = i + 1;
        }
        migrationBuilder.DeleteData(
            table: "Courses",
            keyColumns: new[] { "Id" },
            keyValues: courseKeyValues
        );
    }
}
