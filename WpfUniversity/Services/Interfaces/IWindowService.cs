using System.Threading.Tasks;
using UniversityDataLayer.Entities;

namespace WpfUniversity.Services.Interfaces;

public interface IWindowService
{
    void OpenGroupsWindow(Course selectedCourse);
    void OpenAddCourseWindow();
    void OpenEditCourseWindow(Course course);

    bool ShowConfirmationDialog(string message, string title);
    void ShowMessageDialog(string message, string title);

    void OpenAddGroupWindow(int courseId);
    void OpenEditGroupWindow(Group group);
    void OpenStudentsWindow(Group group);

    void OpenAddStudentWindow(Group group);
    void OpenEditStudentWindow(Student student);
    Task<string> ShowOpenFileDialogAsync(string filter, string title);
    Task<string> ShowSaveFileDialogAsync(string filter, string title);

    void OpenTeachersWindow();
    bool OpenTeacherDialog(Teacher teacher, string title);
}
