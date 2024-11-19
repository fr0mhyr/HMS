using HMS.Domain;

namespace HMS.Application.Common;

public interface IBookingRepository
{
    IList<Booking> GetBookings(string hotelId, string roomType, DateTime startDate, DateTime endDate);
}