using ErrorOr;
using HMS.Application.Common;
using HMS.Domain;

namespace HMS.Application;

public class HotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IBookingRepository _bookingRepository;
    
    public HotelService(IHotelRepository hotelRepository, IBookingRepository bookingRepository)
    {
        _hotelRepository = hotelRepository;
        _bookingRepository = bookingRepository;
    }

    public ErrorOr<int> Availability(string hotelId, DateTime startDate, DateTime endDate, string roomType)
    {
        var hotel = _hotelRepository.GetById(hotelId);
        
        if (hotel == null)
            return Error.NotFound($"No hotel found with id {hotelId}");

        var roomCount = hotel.Rooms?.Count(r=>r.RoomType == roomType) ?? 0;
        
        if(roomCount == 0)
            return Error.NotFound($"The hotel does not have rooms of a {roomType} type");
        
        var bookings = _bookingRepository.GetBookings(hotelId, roomType, startDate, endDate);

        if (bookings.Count == 0)
            return roomCount;
        
        return roomCount - bookings.Count;
    }
}