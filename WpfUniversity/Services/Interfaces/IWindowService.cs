using UniversityDataLayer.Entities;

namespace WpfUniversity.Services.Interfaces;

public interface IWindowService
{
    void OpenGroupsWindow(Course selectedCourse);
    void OpenAddCourseWindow();
    void OpenEditCourseWindow(Course course);

    bool ShowConfirmationDialog(string message, string title);
    void ShowErrorDialog(string message, string title);

    void OpenAddGroupWindow(int courseId);
    void OpenEditGroupWindow(Group group);
    void OpenStudentsWindow(Group group);

    void OpenAddStudentWindow(Group group);
    void OpenEditStudentWindow(Student student);
}
