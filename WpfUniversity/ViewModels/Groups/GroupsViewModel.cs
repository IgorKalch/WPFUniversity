using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using UniversityDataLayer.Entities;
using WpfUniversity.Commands;
using WpfUniversity.Services.Interfaces;

namespace WpfUniversity.ViewModels.Groups;
public class GroupsViewModel : ViewModelBase
{
    private readonly IGroupService _groupService;
    private readonly ITeacherService _teacherService;
    private readonly IWindowService _windowService;

    public GroupsViewModel(IGroupService groupService, IWindowService windowService, ITeacherService teacherService, Course course)
    {
        _groupService = groupService;
        _windowService = windowService;
        _teacherService = teacherService;

        Course = course;
        Groups = new ObservableCollection<Group>();
        LoadGroups();

        LoadGroupsCommand = new AsyncRelayCommand(LoadGroups);
        NextPageGroupsCommand = new RelayCommand(NextPageGroups, () => CanGoToNextPageGroups);
        PreviousPageGroupsCommand = new RelayCommand(PreviousPageGroups, () => CanGoToPreviousPageGroups);

        AddGroupCommand = new RelayCommand(AddGroup);
        EditGroupCommand = new RelayCommand(EditGroup, () => IsGroupSelected);
        RemoveGroupCommand = new RelayCommand(RemoveGroup, () => CanRemoveGroup);

        OpenStudentsCommand = new RelayCommand<Group>(OpenStudents);

        SortCommand = new RelayCommand<DataGridSortingEventArgs>(OnSortCommandExecuted);
    }
    public Course Course { get; }

    public ObservableCollection<Group> Groups { get; set; }

    private Group _selectedGroup;
    public Group SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (SetProperty(ref _selectedGroup, value))
            {
                OnPropertyChanged(nameof(IsGroupSelected));
                OnPropertyChanged(nameof(CanRemoveGroup));

                ((RelayCommand)EditGroupCommand).RaiseCanExecuteChanged();
                ((RelayCommand)RemoveGroupCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsGroupSelected => SelectedGroup != null;

    public bool CanRemoveGroup
    {
        get
        {
            if (SelectedGroup == null)
                return false;

            var hasStudents = SelectedGroup.Students?.Any() ?? false;

            return !hasStudents;
        }
    }

    private string _sortColumn;
    private bool _sortAscending = true;

    public string SortColumn
    {
        get => _sortColumn;
        set
        {
            if (SetProperty(ref _sortColumn, value))
            {
                CurrentPageGroups = 1;
                UpdateGroupsCollection();
            }
        }
    }

    public bool SortAscending
    {
        get => _sortAscending;
        set
        {
            if (SetProperty(ref _sortAscending, value))
            {
                CurrentPageGroups = 1;
                UpdateGroupsCollection();
            }
        }
    }

    private int _currentPageGroups = 1;
    private int _itemsPerPageGroups = 20;
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

    public ICommand AddGroupCommand { get; }
    public ICommand EditGroupCommand { get; }
    public ICommand RemoveGroupCommand { get; }

    public ICommand OpenStudentsCommand { get; }

    public ICommand SortCommand { get; }

    public async Task LoadGroups()
    {
        await _groupService.LoadGroupsByCourseId(Course.Id);

        _totalGroups = _groupService.Groups.Count;
        UpdateGroupsCollection();
    }

    public async Task LoadTeachers()
    {
        await _teacherService.GetAllTeachersByCourseIdAsync(Course.Id);
    }

    private void UpdateGroupsCollection()
    {
        IEnumerable<Group> sortedGroups = _groupService.Groups;

        if (!string.IsNullOrEmpty(SortColumn))
        {
            switch (SortColumn)
            {
                case "Id":
                    sortedGroups = SortAscending ? sortedGroups.OrderBy(g => g.Id) : sortedGroups.OrderByDescending(g => g.Id);
                    break;
                case "Name":
                    sortedGroups = SortAscending ? sortedGroups.OrderBy(g => g.Name) : sortedGroups.OrderByDescending(g => g.Name);
                    break;
                case "Teacher.FullName":
                    sortedGroups = SortAscending ? sortedGroups.OrderBy(g => g.Techer.FullName) : sortedGroups.OrderByDescending(g => g.Techer.FullName);
                    break;
                default:
                    break;
            }
        }

        var pagedGroups = sortedGroups
            .Skip((_currentPageGroups - 1) * _itemsPerPageGroups)
            .Take(_itemsPerPageGroups)
            .ToList();

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

    private void AddGroup()
    {
        _windowService.OpenAddGroupWindow(Course.Id);
        LoadGroupsCommand.Execute(null);
    }

    private void EditGroup()
    {
        if (SelectedGroup == null)
            return;

        _windowService.OpenEditGroupWindow(SelectedGroup);
        LoadGroupsCommand.Execute(null);
    }

    private void RemoveGroup()
    {
        if (SelectedGroup == null)
            return;

        var confirmation = _windowService.ShowConfirmationDialog("Are you sure you want to remove this group?", "Remove Confirmation");
        if (confirmation)
        {
            _groupService.Delete(SelectedGroup);
            LoadGroupsCommand.Execute(null);
        }
    }

    private void OpenStudents(Group group)
    {
        if (group == null)
            return;

        _windowService.OpenStudentsWindow(group);
    }

    private void OnSortCommandExecuted(DataGridSortingEventArgs e)
    {
        var column = e.Column;

        e.Handled = true;

        var sortMemberPath = column.SortMemberPath;
        if (string.IsNullOrEmpty(sortMemberPath))
        {
            return;
        }

        if (SortColumn == sortMemberPath)
        {
            SortAscending = !SortAscending;
        }
        else
        {
            SortColumn = sortMemberPath;
            SortAscending = true;
        }

        column.SortDirection = SortAscending ? ListSortDirection.Ascending : ListSortDirection.Descending;

        UpdateGroupsCollection();
    }
}
