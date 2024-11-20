using ErrorOr;
using HMS.Application.Common;
using HMS.Application.Extensions;

namespace HMS.Application;

public class HotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IDateService _dateService;

    public HotelService(IHotelRepository hotelRepository, IBookingRepository bookingRepository,
        IDateService dateService)
    {
        _hotelRepository = hotelRepository;
        _bookingRepository = bookingRepository;
        _dateService = dateService;
    }

    public ErrorOr<int> Availability(string hotelId, DateTime startDate, DateTime endDate, string roomType)
    {
        var hotel = _hotelRepository.GetById(hotelId);

        if (hotel == null)
            return Error.NotFound($"No hotel found with id {hotelId}");

        var roomCount = hotel.Rooms.Count(r => r.RoomType == roomType);

        if (roomCount == 0)
            return Error.NotFound($"The hotel does not have rooms of a {roomType} type");

        var bookings = _bookingRepository.GetBookings(hotelId, roomType, startDate, endDate);

        if (bookings.Count == 0)
            return roomCount;

        return roomCount - bookings.MinimumRoomsDemand();
    }

    public ErrorOr<IList<RoomTypeAvailabilityRange>> Search(string hotelId, string roomType, int days)
    {
        var hotel = _hotelRepository.GetById(hotelId);

        if (hotel == null)
            return Error.NotFound($"No hotel found with id {hotelId}");

        var roomCount = hotel.Rooms.Count(r => r.RoomType == roomType);

        if (roomCount == 0)
            return Error.NotFound($"The hotel does not have rooms of a {roomType} type");

        DateTime startDate = _dateService.NowDate;
        DateTime endDate = _dateService.NowDate.AddDays(days - 1);

        var bookings = _bookingRepository.GetBookings(hotelId, roomType, startDate, endDate);

        if (bookings.Count == 0)
            return new List<RoomTypeAvailabilityRange>
            {
                new RoomTypeAvailabilityRange(_dateService.NowDate, _dateService.NowDate.AddDays(days - 1), roomCount),
            };

        var roomAvailability = Enumerable.Repeat(roomCount, days).ToArray();

        foreach (var booking in bookings)
        {
            var startIndex = Math.Max(0, (booking.Arrival - startDate).Days);
            var endIndex = Math.Min(days, (booking.Departure - startDate).Days + 1);

            for (var i = startIndex; i < endIndex; i++)
                roomAvailability[i]--;
        }

        var result = new List<RoomTypeAvailabilityRange>();

        int? currentAvailability = null;
        var rangeStart = startDate;

        for (int i = 0; i < days; i++)
        {
            if (roomAvailability[i] <= 0)
            {
                if (currentAvailability.HasValue)
                {
                    result.Add(new RoomTypeAvailabilityRange(rangeStart, startDate.AddDays(i - 1),
                        currentAvailability.Value));
                    currentAvailability = null;
                }

                continue;
            }

            if (currentAvailability == null)
            {
                currentAvailability = roomAvailability[i];
                rangeStart = startDate.AddDays(i);
            }
            else if (currentAvailability != roomAvailability[i])
            {
                result.Add(new RoomTypeAvailabilityRange(rangeStart, startDate.AddDays(i - 1),
                    currentAvailability.Value));
                currentAvailability = roomAvailability[i];
                rangeStart = startDate.AddDays(i);
            }
        }

        if (currentAvailability.HasValue)
        {
            result.Add(new RoomTypeAvailabilityRange(rangeStart, endDate, currentAvailability.Value));
        }

        return result;
    }
}