using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfUniversity.ViewModels.Courses
{
    public class AddCourseFormViewModel : ViewModelBase
    {

        private string _name;
        private string _description;
        private string? _errorMessage;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(CanAddCourse));
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
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public bool CanAddCourse => Name?.Length > 3;

        public ICommand SubmitCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public AddCourseFormViewModel(ICommand submitCommand, ICommand cancelCommand)
        {
            SubmitCommand = submitCommand;
            CancelCommand = cancelCommand;
        }
    }
}
