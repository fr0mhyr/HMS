using Cocona;
using HMS.Application;
using HMS.Client;
using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder();

builder.Services
    .AddClient()
    .AddApplication();

var app = builder.Build();

app.AddCommand((string? hotels, string? bookings, [FromService] IServiceProvider serviceProvider) =>
{
    var queryExecutor = serviceProvider.GetService<QueryExecutor>();

    if (queryExecutor == null)
    {
        Console.WriteLine("Invalid QueryExecutor service");
        return;
    }
    
    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Exiting program.");
            break;
        }
        
        var result = queryExecutor.Execute(input);
        
        Console.WriteLine(result);
    }
});

app.Run();