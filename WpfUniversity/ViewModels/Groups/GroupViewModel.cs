using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using WpfUniversity.Commands;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.ViewModels.Groups;
public class GroupsViewModel : ViewModelBase
{
    private readonly IGroupService _groupService;

    public GroupsViewModel(IGroupService groupService, Course course)
    {
        _groupService = groupService;
        Course = course;

        Groups = new ObservableCollection<Group>();
        LoadGroupsCommand = new AsyncRelayCommand(LoadGroups);

        NextPageGroupsCommand = new RelayCommand(NextPageGroups, () => CanGoToNextPageGroups);
        PreviousPageGroupsCommand = new RelayCommand(PreviousPageGroups, () => CanGoToPreviousPageGroups);

        _ = LoadGroups();
    }
    public Course Course { get; }
    public ObservableCollection<Group> Groups { get; set; }

    private int _currentPageGroups = 1;
    private int _itemsPerPageGroups = 10;
    private int _totalGroups;

    public int CurrentPageGroups
    {
        get => _currentPageGroups;
        set
        {
            if (SetProperty(ref _currentPageGroups, value))
            {
                UpdateGroupsCollection();
            }
        }
    }

    public bool CanGoToNextPageGroups => _currentPageGroups * _itemsPerPageGroups < _totalGroups;
    public bool CanGoToPreviousPageGroups => _currentPageGroups > 1;

    public ICommand LoadGroupsCommand { get; }
    public ICommand NextPageGroupsCommand { get; }
    public ICommand PreviousPageGroupsCommand { get; }

    public async Task LoadGroups()
    {
        await _groupService.LoadGroupsByCourseId(Course.Id);

        _totalGroups = _groupService.Groups.Count;
        UpdateGroupsCollection();
    }

    private void UpdateGroupsCollection()
    {
        var pagedGroups = _groupService.Groups
            .Skip((_currentPageGroups - 1) * _itemsPerPageGroups)
            .Take(_itemsPerPageGroups);

        Groups.Clear();
        foreach (var group in pagedGroups)
        {
            Groups.Add(group);
        }

        OnPropertyChanged(nameof(CanGoToNextPageGroups));
        OnPropertyChanged(nameof(CanGoToPreviousPageGroups));
    }

    private void NextPageGroups()
    {
        _currentPageGroups++;
        UpdateGroupsCollection();
    }

    private void PreviousPageGroups()
    {
        if (_currentPageGroups > 1)
        {
            _currentPageGroups--;
            UpdateGroupsCollection();
        }
    }
}
