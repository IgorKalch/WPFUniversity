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
            AppHost = CreateHostBuilder().Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var scope = AppHost.Services.CreateScope();

            var mainViewModel = scope.ServiceProvider.GetRequiredService<MainViewModel>();
            var mainWindow = new MainWindow()
            {
                DataContext = mainViewModel
            };
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }

        private IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureServices(ConfigureServices);
        }

        private void ConfigureAppConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            config.SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                 .AddEnvironmentVariables();
        }

        private void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            var configuration = hostContext.Configuration;

            ConfigureDatabase(services, configuration);

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            RegisterServices(services);

            RegisterViewModels(services);

            RegisterDialogs(services);

            RegisterFactories(services);

            RegisterViews(services);
        }

        private void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultString");
            services.AddDbContext<UniversityContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IWindowService, WindowService>();
        }

        private void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<MainViewModel>();
            services.AddTransient<CourseViewModel>();
            services.AddTransient<GroupsViewModel>();
            services.AddTransient<GroupViewModel>();
            services.AddTransient<StudentsViewModel>();
            services.AddTransient<StudentViewModel>();
            services.AddTransient<ConfirmationDialogViewModel>();
            services.AddTransient<ErrorDialogViewModel>();
            services.AddTransient<TeachersViewModel>();
        }

        private void RegisterDialogs(IServiceCollection services)
        {
            services.AddTransient<ConfirmationDialog>();
            services.AddTransient<ErrorDialog>();
        }

        private void RegisterFactories(IServiceCollection services)
        {
            services.AddTransient<IGroupsViewModelFactory, GroupsViewModelFactory>();
            services.AddTransient<IGroupsWindowFactory, GroupsWindowFactory>();
            services.AddTransient<IStudentsViewModelFactory, StudentsViewModelFactory>();
            services.AddTransient<IStudentsWindowFactory, StudentsWindowFactory>();
            services.AddTransient<ITeacherViewModelFactory, TeacherViewModelFactory>();
        }

        private void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<CourseWindow>();
            services.AddTransient<GroupsWindow>();
            services.AddTransient<GroupWindow>();
            services.AddTransient<StudentsWindow>();
            services.AddTransient<StudentWindow>();
            services.AddTransient<TeachersWindow>();
        }
    }
}
