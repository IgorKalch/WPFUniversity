using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvvmCross.Platforms.Wpf.Views;
using System;
using System.IO;
using System.Windows;
using UniversityDataLayer;
using UniversityDataLayer.UnitOfWorks;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.Services.Interfaces;
using WpfUniversity.Services.Students;
using WpfUniversity.ViewModels;
using WpfUniversity.ViewModels.Courses;
using WpfUniversity.ViewModels.Dialogs;
using WpfUniversity.ViewModels.Groups;
using WpfUniversity.ViewModels.Students;
using WpfUniversity.ViewModels.Teachers;
using WpfUniversity.Views.Courses;
using WpfUniversity.Views.Dialogs;
using WpfUniversity.Views.Groups;
using WpfUniversity.Views.Students;
using WpfUniversity.Views.Teachers;
using WpfUniversity.WindowFactories;
using WpfUniversity.WindowFactories.Interfaces;

namespace WpfUniversity
{
    public partial class App : MvxApplication
    {
        public IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                         //.AddJsonFile($"appsettings.json")
                         .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json")
                         .AddEnvironmentVariables();

                })
                .ConfigureServices((hostContext, services) =>
                 {
                     var configuration = hostContext.Configuration;
                     // services.AddDataLayerDependencies(configuration);

                     var conncetionsString = configuration.GetConnectionString("DefaultString");
                     services.AddDbContext<UniversityContext>(opt =>
                     {
                         opt.UseSqlServer(conncetionsString);
                     });

                     services.AddScoped<IUnitOfWork, UnitOfWork>();

                     // Services
                     services.AddScoped<ICourseService, CourseService>();
                     services.AddScoped<IGroupService, GroupService>();
                     services.AddScoped<IStudentService, StudentService>();
                     services.AddScoped<ITeacherService, TeacherService>();
                     services.AddScoped<IWindowService, WindowService>();

                     // ViewModels
                     services.AddTransient<MainViewModel>();
                     services.AddTransient<CourseViewModel>();
                     services.AddTransient<GroupsViewModel>();
                     services.AddTransient<GroupViewModel>();
                     services.AddTransient<StudentsViewModel>();
                     services.AddTransient<StudentViewModel>();
                     services.AddTransient<ConfirmationDialogViewModel>();
                     services.AddTransient<ErrorDialogViewModel>();
                     services.AddTransient<TeachersViewModel>();

                     // Views
                     services.AddTransient<MainWindow>();
                     services.AddTransient<CourseWindow>();
                     services.AddTransient<GroupsWindow>();
                     services.AddTransient<GroupWindow>();
                     services.AddTransient<StudentsWindow>();
                     services.AddTransient<StudentWindow>();
                     services.AddTransient<TeachersWindow>();

                     // Dialogs
                     services.AddTransient<ConfirmationDialog>();
                     services.AddTransient<ErrorDialog>();

                     // Factories
                     services.AddTransient<IGroupsViewModelFactory, GroupsViewModelFactory>();
                     services.AddTransient<IGroupsWindowFactory, GroupsWindowFactory>();
                     services.AddTransient<IStudentsViewModelFactory, StudentsViewModelFactory>();
                     services.AddTransient<IStudentsWindowFactory, StudentsWindowFactory>();
                     services.AddTransient<ITeacherViewModelFactory, TeacherViewModelFactory>();
                 })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var starupForm = AppHost!.Services.GetRequiredService<MainWindow>();
            starupForm.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();

            base.OnExit(e);
        }
    }
}
