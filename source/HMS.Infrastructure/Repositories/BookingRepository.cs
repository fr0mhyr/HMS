using HMS.Application.Common;
using HMS.Domain;

namespace HMS.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    public IList<Booking> GetBookings(string hotelId, string roomType, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }
}