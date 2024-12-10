using MigraDocCore.DocumentObjectModel;
using MigraDocCore.Rendering;
using System;
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
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace WpfUniversity.ViewModels.Groups;
public class GroupsViewModel : ViewModelBase
{
    private readonly IGroupService _groupService;
    private readonly ITeacherService _teacherService;
    private readonly IWindowService _windowService;

    private int _currentPageGroups = 1;
    private int _itemsPerPageGroups = 20;
    private int _totalGroups;
    private Group _selectedGroup;

    private string _sortColumn;
    private bool _sortAscending = true;
    private bool _isBusy;

    public GroupsViewModel(IGroupService groupService, IWindowService windowService, ITeacherService teacherService)
    {
        _groupService = groupService;
        _windowService = windowService;
        _teacherService = teacherService;

        Groups = new ObservableCollection<Group>();

        LoadGroupsCommand = new AsyncRelayCommand(LoadGroups);
        NextPageGroupsCommand = new RelayCommand(NextPageGroups, () => CanGoToNextPageGroups);
        PreviousPageGroupsCommand = new RelayCommand(PreviousPageGroups, () => CanGoToPreviousPageGroups);

        AddGroupCommand = new RelayCommand(AddGroup);
        EditGroupCommand = new RelayCommand(EditGroup, () => IsGroupSelected);
        RemoveGroupCommand = new RelayCommand(RemoveGroup, () => CanRemoveGroup);

        OpenStudentsCommand = new RelayCommand<Group>(OpenStudents);

        SortCommand = new RelayCommand<DataGridSortingEventArgs>(OnSortCommandExecuted);

        ExportToDocxCommand = new AsyncRelayCommand(ExportToDocxAsync, () => !IsBusy);
        ExportToPdfCommand = new AsyncRelayCommand(ExportToPdfAsync, () => !IsBusy);

    }

    #region Public Properties
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                ((AsyncRelayCommand)ExportToDocxCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)ExportToPdfCommand).RaiseCanExecuteChanged();
            }
        }
    }
    public Course Course { get; set; }

    public ObservableCollection<Group> Groups { get; set; }

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

    public int PageSizeGroups
    {
        get => _itemsPerPageGroups;
        set
        {
            if (SetProperty(ref _itemsPerPageGroups, value))
            {
                UpdateGroupsCollection();
            }
        }
    }

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

    #endregion

    #region Commands

    public ICommand LoadGroupsCommand { get; }
    public ICommand NextPageGroupsCommand { get; }
    public ICommand PreviousPageGroupsCommand { get; }

    public ICommand AddGroupCommand { get; }
    public ICommand EditGroupCommand { get; }
    public ICommand RemoveGroupCommand { get; }

    public ICommand OpenStudentsCommand { get; }

    public ICommand SortCommand { get; }

    public ICommand ExportToDocxCommand { get; }
    public ICommand ExportToPdfCommand { get; }

    #endregion

    public void Initialize(Course course)
    {
        Course = course;
        LoadGroupsCommand.Execute(this);

    }

    public async Task LoadGroups()
    {
        if (Course == null)
            return;

        await _groupService.LoadGroupsByCourseId(Course.Id);

        _totalGroups = _groupService.Groups.Count;
        UpdateGroupsCollection();
    }

    public async Task LoadTeachers()
    {
        await _teacherService.GetAllTeachersByCourseIdAsync(Course.Id);
    }

    #region Private Metods
    private async Task ExportToDocxAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            string filePath = await _windowService.ShowSaveFileDialogAsync("DOCX Files (*.docx)|*.docx", "Export Group to DOCX");

            if (string.IsNullOrEmpty(filePath))
                return;

            using (var doc = DocX.Create(filePath))
            {
                var title = doc.InsertParagraph();
                title.AppendLine("Group Report")
                     .FontSize(20)
                     .Bold()
                     .Alignment = Alignment.center;

                doc.InsertParagraph($"Course Name: {Course.Name}")
                   .FontSize(14)
                   .SpacingAfter(10);

                doc.InsertParagraph($"Group Name: {SelectedGroup.Name}")
                   .FontSize(14)
                   .SpacingAfter(20);

                doc.InsertParagraph($"List of Students:")
                   .FontSize(12)
                   .SpacingAfter(10);

                var list = doc.AddList(listType: ListItemType.Numbered);
                foreach (var student in SelectedGroup.Students)
                {
                    doc.AddListItem(list, $"{student.FirstName} {student.LastName}");
                }
                doc.InsertList(list);

                doc.Save();
            }

            _windowService.ShowMessageDialog("Group exported to DOCX successfully.", "Export Complete");
        }
        catch (Exception ex)
        {
            _windowService.ShowMessageDialog($"Error exporting to DOCX: {ex.Message}", "Export Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExportToPdfAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            string filePath = await _windowService.ShowSaveFileDialogAsync("PDF Files (*.pdf)|*.pdf", "Export Group to PDF");

            if (string.IsNullOrEmpty(filePath))
                return;

            var document = new MigraDocCore.DocumentObjectModel.Document();
            document.Info.Title = "Group Report";
            document.Info.Subject = $"Course: {Course.Name}, Group: {SelectedGroup.Name}";
            document.Info.Author = "WpfUniversity Application";

            var style = document.Styles["Normal"];
            style.Font.Name = "Verdana";

            var section = document.AddSection();
            var title = section.AddParagraph("Group Report");
            title.Format.Font.Size = 20;
            title.Format.Font.Bold = true;
            title.Format.Alignment = ParagraphAlignment.Center;

            var course = section.AddParagraph($"Course Name: {Course.Name}");
            course.Format.Font.Size = 14;
            course.Format.SpaceAfter = "0.5cm";

            var group = section.AddParagraph($"Group Name: {SelectedGroup.Name}");
            group.Format.Font.Size = 14;
            group.Format.SpaceAfter = "1cm";

            var list = section.AddParagraph($"List of Students:");
            list.Format.Font.Size = 12;
            list.Format.SpaceAfter = "0.5cm";

            foreach (var student in SelectedGroup.Students)
            {
                var paragraph = section.AddParagraph($"{student.FirstName} {student.LastName}");
                paragraph.Format.Font.Size = 10;
                paragraph.Format.LeftIndent = Unit.FromCentimeter(1);
                paragraph.Format.SpaceAfter = Unit.FromCentimeter(0.2);

                paragraph.Format.ListInfo = new ListInfo
                {
                    ListType = ListType.NumberList1,
                    ContinuePreviousList = true,
                };
            }

            var renderer = new PdfDocumentRenderer(true)
            {
                Document = document
            };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(filePath);

            _windowService.ShowMessageDialog("Group exported to PDF successfully.", "Export Complete");
        }
        catch (Exception ex)
        {
            _windowService.ShowMessageDialog($"Error exporting to PDF: {ex.Message}", "Export Error");
        }
        finally
        {
            IsBusy = false;
        }
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
    #endregion
}
