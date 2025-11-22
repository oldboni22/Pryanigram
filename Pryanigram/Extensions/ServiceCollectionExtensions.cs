using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pryanigram.ScopedBag.Contract;

namespace Pryanigram.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScopedBag(this IServiceCollection services)
    {
        services.TryAddScoped<IScopedBag, ScopedBag.Default.ScopedBag>();
        
        return services;
    }

    public static IServiceCollection AddScopedBag<T>(this IServiceCollection services) where T : class, IScopedBag
    {
        services.TryAddScoped<IScopedBag, T>();
        
        return services;
    }
}
