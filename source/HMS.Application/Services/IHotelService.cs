using ErrorOr;
using HMS.Application.Models;

namespace HMS.Application.Services;

public interface IHotelService
{
    ErrorOr<int> Availability(string hotelId, DateTime startDate, DateTime endDate, string roomType);
    ErrorOr<IList<RoomTypeAvailabilityRange>> Search(string hotelId, string roomType, int days);
}