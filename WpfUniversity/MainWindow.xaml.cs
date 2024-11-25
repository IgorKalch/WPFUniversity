using MvvmCross.Platforms.Wpf.Views;
using WpfUniversity.ViewModels;

namespace WpfUniversity;

public partial class MainWindow : MvxWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;

        Loaded += async (s, e) => await viewModel.LoadCourses();
    }
}
