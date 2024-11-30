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

namespace WpfUniversity.ViewModels.Students;

public class StudentsViewModel : ViewModelBase
{
    private readonly IStudentService _studentService;
    private readonly IWindowService _windowService;
    private Student _selectedStudent;
    private string _sortColumn;
    private bool _sortAscending = true;
    private int _currentPageStudents = 1;
    private int _itemsPerPageStudents = 20;
    private int _totalStudents;
    private Group _group;

    public StudentsViewModel(IStudentService studentService, IWindowService windowService, Group group)
    {
        _studentService = studentService;
        _windowService = windowService;
        Students = new ObservableCollection<Student>();

        LoadStudentsCommand = new AsyncRelayCommand(LoadStudents);

        Group = group;
        NextPageStudentsCommand = new RelayCommand(NextPageStudents, () => CanGoToNextPageStudents);
        PreviousPageStudentsCommand = new RelayCommand(PreviousPageStudents, () => CanGoToPreviousPageStudents);

        AddStudentCommand = new RelayCommand(AddStudent);
        EditStudentCommand = new RelayCommand(EditStudent, () => IsStudentSelected);
        RemoveStudentCommand = new RelayCommand(RemoveStudent, () => CanRemoveStudent);

        SortCommand = new RelayCommand<DataGridSortingEventArgs>(OnSortCommandExecuted);
    }

    public ObservableCollection<Student> Students { get; set; }
    public Student SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            if (SetProperty(ref _selectedStudent, value))
            {
                OnPropertyChanged(nameof(IsStudentSelected));
                OnPropertyChanged(nameof(CanRemoveStudent));

                ((RelayCommand)EditStudentCommand).RaiseCanExecuteChanged();
                ((RelayCommand)RemoveStudentCommand).RaiseCanExecuteChanged();
            }
        }
    }
    public bool IsStudentSelected => SelectedStudent != null;
    public bool CanRemoveStudent
    {
        get
        {
            if (SelectedStudent == null)
                return false;

            // TODO: Check in task of some logic

            return true;
        }
    }
    public string SortColumn
    {
        get => _sortColumn;
        set
        {
            if (SetProperty(ref _sortColumn, value))
            {
                CurrentPageStudents = 1;
                UpdateStudentsCollection();
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
                CurrentPageStudents = 1;
                UpdateStudentsCollection();
            }
        }
    }
    public int CurrentPageStudents
    {
        get => _currentPageStudents;
        set
        {
            if (SetProperty(ref _currentPageStudents, value))
            {
                UpdateStudentsCollection();
            }
        }
    }
    public bool CanGoToNextPageStudents => _currentPageStudents * _itemsPerPageStudents < _totalStudents;
    public bool CanGoToPreviousPageStudents => _currentPageStudents > 1;
    public int StudentsCount => _totalStudents;
    public Group Group
    {
        get => _group;
        set
        {
            if (SetProperty(ref _group, value))
            {
                LoadStudentsCommand.Execute(null);
            }
        }
    }
    public async Task LoadStudents()
    {
        if (Group == null)
            return;

        await _studentService.LoadStudentsByGroup(Group.Id);

        _totalStudents = _studentService.Students.Count;
        UpdateStudentsCollection();
    }

    #region Commands
    public ICommand LoadStudentsCommand { get; }
    public ICommand NextPageStudentsCommand { get; }
    public ICommand PreviousPageStudentsCommand { get; }

    public ICommand AddStudentCommand { get; }
    public ICommand EditStudentCommand { get; }
    public ICommand RemoveStudentCommand { get; }

    public ICommand SortCommand { get; }
    #endregion


    private void UpdateStudentsCollection()
    {
        IEnumerable<Student> sortedStudents = _studentService.Students;

        if (!string.IsNullOrEmpty(SortColumn))
        {
            switch (SortColumn)
            {
                case "Id":
                    sortedStudents = SortAscending ? sortedStudents.OrderBy(s => s.Id) : sortedStudents.OrderByDescending(s => s.Id);
                    break;
                case "FullName":
                    sortedStudents = SortAscending ? sortedStudents.OrderBy(s => s.FirstName) : sortedStudents.OrderByDescending(s => s.FirstName);
                    break;
                case "Email":
                    sortedStudents = SortAscending ? sortedStudents.OrderBy(s => s.LastName) : sortedStudents.OrderByDescending(s => s.LastName);
                    break;
                default:
                    break;
            }
        }

        var pagedStudents = sortedStudents
            .Skip((_currentPageStudents - 1) * _itemsPerPageStudents)
            .Take(_itemsPerPageStudents)
            .ToList();

        Students.Clear();
        foreach (var student in pagedStudents)
        {
            Students.Add(student);
        }

        OnPropertyChanged(nameof(CanGoToNextPageStudents));
        OnPropertyChanged(nameof(CanGoToPreviousPageStudents));
    }

    private void NextPageStudents()
    {
        _currentPageStudents++;
        UpdateStudentsCollection();
    }

    private void PreviousPageStudents()
    {
        if (_currentPageStudents > 1)
        {
            _currentPageStudents--;
            UpdateStudentsCollection();
        }
    }

    private void AddStudent()
    {
        _windowService.OpenAddStudentWindow(Group);
        LoadStudentsCommand.Execute(null);
    }

    private void EditStudent()
    {
        if (SelectedStudent == null)
            return;

        _windowService.OpenEditStudentWindow(SelectedStudent);
        LoadStudentsCommand.Execute(null);
    }

    private void RemoveStudent()
    {
        if (SelectedStudent == null)
            return;

        var confirmation = _windowService.ShowConfirmationDialog("Are you sure you want to remove this student?", "Remove Confirmation");
        if (confirmation)
        {
            _studentService.Delete(SelectedStudent);
            LoadStudentsCommand.Execute(null);
        }
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

        UpdateStudentsCollection();
    }

    public void SetGroup(Group group)
    {
        Group = group;
        OnPropertyChanged(nameof(Group));
    }
}
