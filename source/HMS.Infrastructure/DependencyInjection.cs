using HMS.Application.Common;
using HMS.Infrastructure.Common;
using HMS.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IDateService, DateService>();
        
        services.AddTransient<IHotelRepository, HotelRepository>();
        services.AddTransient<IBookingRepository, BookingRepository>();
        
        return services;
    }
}
