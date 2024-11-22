using System.Reflection;
using HMS.Application.Common;
using HMS.Client.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Client;

public static class DependencyInjection
{
    public static IServiceCollection AddClient(this IServiceCollection services, string hotelFileName,
        string bookingFileName)
    {
        AddQueries(services);

        services.AddTransient<QueryExecutor>();
        services.AddTransient<Application>();
        services.AddSingleton<RepositoryConfiguration>(s => new RepositoryConfiguration()
        {
            HotelsFileName = hotelFileName,
            BookingsFileName = bookingFileName
        });

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