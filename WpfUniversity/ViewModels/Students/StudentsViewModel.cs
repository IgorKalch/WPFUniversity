using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    private int _studentsCount;
    private Group _group;
    private bool _isBusy;

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

        ExportStudentsCommand = new AsyncRelayCommand(ExportStudentsAsync, () => !IsBusy);
        ImportStudentsCommand = new AsyncRelayCommand(ImportStudentsAsync, () => !IsBusy);
    }
    #region Public Properties

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                ((AsyncRelayCommand)ExportStudentsCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)ImportStudentsCommand).RaiseCanExecuteChanged();
            }
        }
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
    public int PageSizeStudents
    {
        get => _itemsPerPageStudents;
        set
        {
            if (SetProperty(ref _itemsPerPageStudents, value))
            {
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
    public int StudentsCount
    {
        get => _studentsCount;
        set => SetProperty(ref _studentsCount, value);
    }

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
    #endregion

    #region Commands
    public ICommand LoadStudentsCommand { get; }
    public ICommand NextPageStudentsCommand { get; }
    public ICommand PreviousPageStudentsCommand { get; }

    public ICommand AddStudentCommand { get; }
    public ICommand EditStudentCommand { get; }
    public ICommand RemoveStudentCommand { get; }

    public ICommand SortCommand { get; }

    public ICommand ExportStudentsCommand { get; }
    public ICommand ImportStudentsCommand { get; }
    #endregion

    #region Private Metods
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

    private async Task ExportStudentsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            string filePath = await _windowService.ShowSaveFileDialogAsync("CSV Files (*.csv)|*.csv", "Export Students");

            if (string.IsNullOrEmpty(filePath))
                return;

            var students = _studentService.GetStudentsByGroup(Group.Id);

            if (students == null || !students.Any())
            {
                _windowService.ShowMessageDialog("No students to export.", "Export");
                return;
            }

            var csvContent = GenerateCsv(students);

            await System.IO.File.WriteAllTextAsync(filePath, csvContent);

            _windowService.ShowMessageDialog("Students exported successfully.", "Export");
        }
        catch (Exception ex)
        {
            _windowService.ShowMessageDialog($"Error exporting students: {ex.Message}", "Export Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string GenerateCsv(IEnumerable<Student> students)
    {
        var csv = new StringBuilder();
        csv.AppendLine("FirstName,LastName");

        foreach (var student in students)
        {
            string firstName = EscapeCsvField(student.FirstName);
            string lastName = EscapeCsvField(student.LastName);

            csv.AppendLine($"{firstName},{lastName}");
        }

        return csv.ToString();
    }

    private string EscapeCsvField(string field)
    {
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            field = field.Replace("\"", "\"\"");
            field = $"\"{field}\"";
        }
        return field;
    }

    private async Task ImportStudentsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            string filePath = await _windowService.ShowOpenFileDialogAsync("CSV Files (*.csv)|*.csv", "Import Students");

            if (string.IsNullOrEmpty(filePath))
                return;

            string csvContent = await System.IO.File.ReadAllTextAsync(filePath);

            var importedStudents = ParseCsv(csvContent);

            if (importedStudents == null || !importedStudents.Any())
            {
                _windowService.ShowMessageDialog("No valid students found in the CSV file.", "Import");
                return;
            }

            bool confirm = _windowService.ShowConfirmationDialog("Importing will remove all existing students in this group. Do you want to continue?", "Import Confirmation");
            if (!confirm)
                return;

            _studentService.ClearStudentsInGroup(Group.Id);

            foreach (var student in importedStudents)
            {
                _studentService.AddStudentToGroup(Group.Id, student);
            }

            await LoadStudents();

            _windowService.ShowMessageDialog("Students imported successfully.", "Import");
        }
        catch (Exception ex)
        {
            _windowService.ShowMessageDialog($"Error importing students: {ex.Message}", "Import Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private List<Student> ParseCsv(string csvContent)
    {
        var students = new List<Student>();
        var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
            return students;

        var headers = lines[0].Split(',');

        int firstNameIndex = Array.IndexOf(headers, "FirstName");
        int lastNameIndex = Array.IndexOf(headers, "LastName");

        if (firstNameIndex == -1 || lastNameIndex == -1)
            throw new FormatException("CSV headers are missing or incorrect.");

        for (int i = 1; i < lines.Length; i++)
        {
            var fields = SplitCsvLine(lines[i]);

            if (fields.Length != headers.Length)
                continue;

            var firstName = UnescapeCsvField(fields[firstNameIndex]);
            var lastName = UnescapeCsvField(fields[lastNameIndex]);

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                continue;

            var student = new Student
            {
                GroupId = Group.Id,
                FirstName = firstName,
                LastName = lastName,
            };

            students.Add(student);
        }

        return students;
    }

    private string[] SplitCsvLine(string line)
    {
        var fields = new List<string>();
        bool inQuotes = false;
        StringBuilder field = new StringBuilder();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    field.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }

        fields.Add(field.ToString());
        return fields.ToArray();
    }

    private string UnescapeCsvField(string field)
    {
        if (field.StartsWith("\"") && field.EndsWith("\""))
        {
            field = field.Substring(1, field.Length - 2).Replace("\"\"", "\"");
        }
        return field;
    }

    #endregion
}
