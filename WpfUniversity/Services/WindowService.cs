using UniversityDataLayer.Entities;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity.Services;

public class WindowService : IWindowService
{
    private readonly IGroupsWindowFactory _groupsWindowFactory;

    public WindowService(IGroupsWindowFactory groupsWindowFactory)
    {
        _groupsWindowFactory = groupsWindowFactory;
    }

    public void OpenGroupsWindow(Course selectedCourse)
    {
        var groupsWindow = _groupsWindowFactory.Create(selectedCourse);
        groupsWindow.Show();
    }
}
