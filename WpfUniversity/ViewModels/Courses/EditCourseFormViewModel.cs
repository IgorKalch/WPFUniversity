using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfUniversity.ViewModels.Courses
{
    public class EditCourseFormViewModel : ViewModelBase
    {

        private string _name;
        private string _description;
        private string? _errorMessage;
        private bool _isSubmitting;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(CanEditCourse));
            }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string? ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }

        public bool IsSubmitting
        {
            get
            {
                return _isSubmitting;
            }
            set
            {
                _isSubmitting = value;
                OnPropertyChanged(nameof(IsSubmitting));
            }
        }

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public bool CanEditCourse => Name?.Length > 3;

        public ICommand SubmitCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public EditCourseFormViewModel(ICommand submitCommand, ICommand cancelCommand)
        {
            SubmitCommand = submitCommand;
            CancelCommand = cancelCommand;
        }
    }
}
