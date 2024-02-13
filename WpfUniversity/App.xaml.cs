using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using UniversityDataLayer.Extensions;
using WpfUniversity.ViewModels.Courses;
using MvvmCross.Platforms.Wpf.Views;
using WpfUniversity.Services;
using WpfUniversity.Services.Courses;
using WpfUniversity.ViewModels;
using System;
using UniversityDataLayer.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using UniversityDataLayer;

namespace WpfUniversity
{
    public partial class App : MvxApplication
    {
        public  IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                
                .ConfigureServices((hostContext, services) =>
                 {
                    var configuration = hostContext.Configuration;
                    services.AddDataLayerDependencies(configuration);

                    services.AddSingleton<ModalNavigationService>();
                    services.AddSingleton<CourseService>();
                    services.AddSingleton<SelectedCourseService>();

                    services.AddSingleton<CourseViewModel>(CreateCourseViewModel);
                    services.AddSingleton<MainWindowViewModel>();


                    services.AddSingleton<MainWindow>((services) => new MainWindow()
                    {
                        DataContext = services.GetRequiredService<MainWindowViewModel>()
                    });

                }).ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                         .AddEnvironmentVariables();
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

        protected override  async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();

            base.OnExit(e);
        }

        private CourseViewModel CreateCourseViewModel(IServiceProvider services)
        {
            return CourseViewModel.LoadViewModel(
                services.GetRequiredService<CourseService>(),
                services.GetRequiredService<SelectedCourseService>(),
                services.GetRequiredService<ModalNavigationService>());
        }
    }
}
