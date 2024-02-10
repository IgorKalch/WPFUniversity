using MvvmCross.Platforms.Wpf.Views;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.ViewModels.Courses;

namespace WpfUniversity
{
    public partial class MainWindow_old : MvxWindow
    {
        private readonly IUnitOfWork _unitOfWork;

        public MainWindow_old(IUnitOfWork unitOfWork) 
        {
            //InitializeComponent();

            _unitOfWork = unitOfWork;

           //DataContext = new CourseViewModel(_unitOfWork);
        }
    }
}
