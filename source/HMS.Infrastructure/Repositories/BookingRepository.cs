using HMS.Application.Common;
using HMS.Domain;
using HMS.Infrastructure.Common;

namespace HMS.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly RepositoryConfiguration _configuration;
    private readonly Lazy<List<Booking>> _bookings;

    public BookingRepository(RepositoryConfiguration configuration)
    {
        _configuration = configuration;
        _bookings = new Lazy<List<Booking>>(LoadBookingsFromFile);
    }

    private IReadOnlyList<Booking> Bookings => _bookings.Value;

    public IList<Booking> GetBookings(string hotelId, string roomType, DateTime startDate, DateTime endDate)
    {
        return Bookings
            .Where(b => b.HotelId == hotelId && b.RoomType == roomType && b.Arrival <= endDate && b.Departure >= startDate)
            .ToList();
    }

    private List<Booking> LoadBookingsFromFile()
    {
        var data = JsonFileReader.Read<List<Booking>>(_configuration.BookingsFileName);

        if (data == null)
            throw new Exception("No booking file found or it is empty.");

        return data;
    }
}