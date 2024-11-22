using HMS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IHotelService, HotelService>();
        
        return services;
    }
}