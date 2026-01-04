using Confluent.Kafka;
using DbSync.TwoWays.Application.DbServices;
using DbSync.TwoWays.Application.Models;
using DbSync.TwoWays.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbSync.TwoWays.Application;

public static class ServiceRegistration
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IMsSqlDbService, MsSqlDbService>();
        services.AddSingleton<IPostgreSqlDbService, PostgreSqlDbService>();
    }

    public static void RegisterConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<SyncService1>();
        services.AddSingleton<SyncService2>();
        services.AddSingleton<SyncService3>();
        services.AddSingleton<SyncService4>();
    }
}
