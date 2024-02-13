using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UniversityDataLayer.UnitOfWorks;

namespace UniversityDataLayer.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddDataLayerDependencies (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UniversityContext>(opt =>
        {
            var conncetionsString = configuration.GetConnectionString("DefaultString");
            opt.UseSqlServer(conncetionsString);
        });

        services.AddSingleton<IUnitOfWork, UnitOfWork>(x => new UnitOfWork(x.GetRequiredService<UniversityContext>()));
    }
}
