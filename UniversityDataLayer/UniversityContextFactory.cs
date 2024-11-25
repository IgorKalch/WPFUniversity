using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UniversityDataLayer;

public class UniversityContextFactory : IDesignTimeDbContextFactory<UniversityContext>
{
    public UniversityContext CreateDbContext(string[] args)
    {
        // Determine the environment (optional)
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

        // Build configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: false)
            .Build();

        // Retrieve the connection string
        var connectionString = configuration.GetConnectionString("DefaultString")
            ?? throw new InvalidOperationException("Connection string '' not found.");

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<UniversityContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new UniversityContext(optionsBuilder.Options);
    }
}
