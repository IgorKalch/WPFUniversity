using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using UniversityDataLayer.Extensions;
using WpfUniversity.ViewModels.Courses;
using MvvmCross.Platforms.Wpf.Views;

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
                    services.AddSingleton<MainWindow>();
                    services.AddDataLayerDependencies(configuration);
                    services.AddTransient<CourseViewModel>();

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
    }
}
