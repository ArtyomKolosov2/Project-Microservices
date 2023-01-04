﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ordering.API.Extensions;

public static class WebApplicationExtensions
{
    private const int DefaultRetryCount = 50;

    public static WebApplication MigrateDatabase<TContext>(this WebApplication webApplication, Action<TContext, IServiceProvider> seeder, int? retry = 0) where TContext : DbContext
    {
        var retryForAvailability = retry < 0 ? DefaultRetryCount : retry ?? DefaultRetryCount;

        using var scope = webApplication.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<TContext>>();
        var context = services.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

            InvokeSeeder(seeder, context, services);

            logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);

            if (retryForAvailability < DefaultRetryCount)
            {
                retryForAvailability++;
                Thread.Sleep(2000);
                MigrateDatabase(webApplication, seeder, retryForAvailability);
            }
        }

        return webApplication;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services) where TContext : DbContext
    {
        context.Database.Migrate();
        seeder(context, services);
    }
}