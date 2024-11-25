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
using WpfUniversity.ViewModels;
using WpfUniversity.ViewModels.Groups;
using WpfUniversity.Views.Groups;
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

                     services.AddTransient<IUnitOfWork, UnitOfWork>();

                     // Services
                     services.AddScoped<ICourseService, CourseService>();
                     services.AddScoped<IGroupService, GroupService>();
                     services.AddTransient<IWindowService, WindowService>();

                     // ViewModels
                     services.AddTransient<MainViewModel>();
                     services.AddTransient<GroupsViewModel>();

                     // Views
                     services.AddTransient<MainWindow>();
                     services.AddTransient<GroupsWindow>();

                     services.AddTransient<IGroupsViewModelFactory, GroupsViewModelFactory>();
                     services.AddTransient<IGroupsWindowFactory, GroupsWindowFactory>();
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
