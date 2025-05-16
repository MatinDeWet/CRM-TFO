using System.Reflection;
using CRM.Application;
using CRM.Persistence;
using CRM.Persistence.Data.Context;
using CRM.WebApi.Common.Filters;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddFastEndpoints()
            .SwaggerDocument();

        builder.Services.AddDataIdentity();
        builder.Services.AddApplicationMessaging();

        builder.Services
            .AddDbContext<CrmContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("CrmConnection"),
                    opt => opt.MigrationsAssembly(typeof(CrmContext).GetTypeInfo().Assembly.GetName().Name));

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
            })
            .AddDataSecurity()
            .AddRepos(typeof(CrmContext))
            .AddRepoLocks(typeof(CrmContext));

        WebApplication app = builder.Build();

        app.UseDefaultExceptionHandler()
            .UseFastEndpoints(c => c.Endpoints.Configurator = 
            ep => ep.Options(b => b.AddEndpointFilter<UserIdentifierFilter>())
            )
        .UseSwaggerGen();

        if (builder.Environment.IsDevelopment())
        {
            //ApplyDbMigrations(app);
        }

        app.Run();
    }

    internal static void ApplyDbMigrations(IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();

        if (serviceScope.ServiceProvider.GetRequiredService<CrmContext>().Database.GetPendingMigrations().Any())
        {
            serviceScope.ServiceProvider.GetRequiredService<CrmContext>().Database.Migrate();
        }
    }
}
