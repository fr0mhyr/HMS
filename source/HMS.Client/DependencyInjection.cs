using System.Reflection;
using HMS.Client.Queries.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Client;

public static class DependencyInjection
{
    public static IServiceCollection AddClient(this IServiceCollection services)
    {
        AddQueries(services);

        services.AddTransient<QueryExecutor>();
        
        return services;
    }

    private static void AddQueries(IServiceCollection services)
    {
        var queriesDictionary = new Dictionary<string, Func<IServiceProvider, IQuery>>();
        
        var queryTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IQuery).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();

        foreach (var queryType in queryTypes)
        {
            var queryName = queryType.Name.Replace("Query", "");
            queriesDictionary[queryName] = sp => (IQuery)sp.GetRequiredService(queryType);
            services.AddTransient(queryType);
        }

        services.AddSingleton(queriesDictionary);
    }
}