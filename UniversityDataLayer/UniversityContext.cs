using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace UniversityDataLayer;

public class UniversityContext : DbContext
{
    public UniversityContext(DbContextOptions<UniversityContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        ApplyEntityConfigurations(modelBuilder);

    }
    private void ApplyEntityConfigurations(ModelBuilder modelBuilder)
    {
        var entityTypeConfigurationType = typeof(IEntityTypeConfiguration<>);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var configurationTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == entityTypeConfigurationType));

            foreach (var configurationType in configurationTypes)
            {
                dynamic configurationInstance = Activator.CreateInstance(configurationType);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
        }
    }
}
