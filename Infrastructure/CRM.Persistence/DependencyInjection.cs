using CRM.Application.Common.Repositories;
using CRM.Persistence.Common.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Persistence;
public static class DependencyInjection
{
    public static IServiceCollection AddDataSecurity(this IServiceCollection services)
    {
        services.AddScoped<AccessRequirements>();

        return services;
    }

    public static IServiceCollection AddRepos(this IServiceCollection services, Type pointer)
    {
        services.Scan(scan => scan
            .FromAssemblies(pointer.Assembly)
            .AddClasses((classes) => classes.AssignableToAny(typeof(ISecureQuery), typeof(ISecureCommand)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddRepoLocks(this IServiceCollection services, Type pointer)
    {
        services.Scan(scan => scan
            .FromAssemblies(pointer.Assembly)
            .AddClasses((classes) => classes.AssignableToAny(typeof(IProtected)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
