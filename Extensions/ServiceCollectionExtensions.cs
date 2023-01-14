using Medallion.Threading.Redis;
using Microsoft.OpenApi.Models;
using ProblemDetailsExample.Jobs;
using StackExchange.Redis;

namespace ProblemDetailsExample.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "Demo API", Version = "v1"});
            c.SwaggerDoc("v2", new OpenApiInfo {Title = "Demo API", Version = "v2"});
        });
    }

    public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        string redisConnectionString = configuration.GetConnectionString("redis") ?? string.Empty;

        services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnectionString; });

        ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);

        RedisDistributedSynchronizationProvider lockProvider = new RedisDistributedSynchronizationProvider(connectionMultiplexer.GetDatabase(), c =>
        {
            c.Expiry(TimeSpan.FromSeconds(10));
            c.BusyWaitSleepTime(TimeSpan.FromMicroseconds(3), TimeSpan.FromMicroseconds(10));
        });

        services.AddSingleton(_ => lockProvider);
    }

    public static IServiceCollection AddBackgroundService(this IServiceCollection services)
    {
        services.AddHostedService<DemoBackgroundService>();

        return services;
    }
}