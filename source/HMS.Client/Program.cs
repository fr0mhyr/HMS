using HMS.Application;
using HMS.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Client;

internal static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Wrong parameters. Expected 4 arguments. --hotels hotels.json --bookings bookings.json");
            return;
        }
        
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddClient(args[1], args[3])
            .AddApplication()
            .AddInfrastructure();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var application = serviceProvider.GetRequiredService<Application>();
        application.Run();
    }
}