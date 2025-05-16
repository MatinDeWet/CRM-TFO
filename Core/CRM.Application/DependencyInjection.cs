using CRM.Application.Common.Behaviors;
using CRM.Application.Common.Security.Contracts;
using CRM.Application.Common.Security.Implementation;
using CRM.Domain.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddDataIdentity(this IServiceCollection services)
    {
        services.AddScoped<IIdentityInfo, IdentityInfo>();
        services.AddScoped<IInfoSetter, InfoSetter>();

        return services;
    }

    public static IServiceCollection AddApplicationMessaging(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
