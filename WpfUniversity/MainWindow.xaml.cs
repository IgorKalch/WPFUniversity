using MvvmCross.Platforms.Wpf.Views;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity
{
    public partial class MainWindow : MvxWindow
    {
        private readonly IUnitOfWork _unitOfWork;

        public MainWindow(IUnitOfWork unitOfWork) 
        {
            InitializeComponent();

            _unitOfWork = unitOfWork;

            DataContext = new CourseViewModel(_unitOfWork);
        }
    }
}
