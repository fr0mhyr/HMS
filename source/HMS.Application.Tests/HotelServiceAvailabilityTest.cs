using ErrorOr;
using FluentAssertions;
using HMS.Application.Common;
using HMS.Domain;
using NSubstitute;
using Xunit;

namespace HMS.Application.Tests;

public class HotelServiceAvailabilityTest
{
    private readonly HotelService _sup;
    private readonly IHotelRepository _hotelRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly DateTime _startDate = new DateTime(2020, 1, 1);
    private readonly DateTime _endDate = new DateTime(2020, 1, 31);

    public HotelServiceAvailabilityTest()
    {
        _hotelRepository = Substitute.For<IHotelRepository>();
        _hotelRepository.GetById("Fake_Hotel").Returns((Hotel?)null);
        _hotelRepository.GetById("H0")
            .Returns(new Hotel()
            {
                Id = "H0",
                Rooms =
                [
                    new Room { RoomType = "RT0" },
                    new Room { RoomType = "RT0" },
                    new Room { RoomType = "RT0" },

                    new Room { RoomType = "RT1" },
                    new Room { RoomType = "RT1" },
                    new Room { RoomType = "RT1" },
                    new Room { RoomType = "RT1" },
                    new Room { RoomType = "RT1" },
                    new Room { RoomType = "RT1" },

                    new Room { RoomType = "RT2" },
                    new Room { RoomType = "RT2" },
                ]
            });

        _bookingRepository = Substitute.For<IBookingRepository>();
        _bookingRepository.GetBookings("H0", "RT0", Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new List<Booking>());
        _bookingRepository.GetBookings("H0", "RT1", _startDate, _endDate)
            .Returns(new List<Booking>()
            {
                new Booking()
                {
                    Arrival = new DateTime(2019, 12, 30),
                    Departure = new DateTime(2020, 1, 15),
                    RoomType = "RT1"
                },
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 1),
                    Departure = new DateTime(2020, 1, 14),
                    RoomType = "RT1"
                },
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 10),
                    Departure = new DateTime(2020, 1, 20),
                    RoomType = "RT1"
                },
                new Booking()
                {
                    Arrival = new DateTime(2019, 12, 1),
                    Departure = new DateTime(2020, 2, 29),
                    RoomType = "RT1"
                },
            });

        _sup = new HotelService(_hotelRepository, _bookingRepository);
    }

    [Fact]
    public void WhenHotelDoesNotExist_ThenShouldReturnError()
    {
        var result = _sup.Availability("Fake_Hotel", _startDate, _endDate, "Fake_Room");

        result.Should().BeOfType<ErrorOr<int>>();
        result.Errors.First().Code.Should().Be("No hotel found with id Fake_Hotel");
    }

    [Fact]
    public void WhenHotelDoesNotHaveSpecificRoomType_ThenShouldReturnError()
    {
        var result = _sup.Availability("H0", _startDate, _endDate, "Fake_Room");

        result.Should().BeOfType<ErrorOr<int>>();
        result.Errors.First().Code.Should().Be("The hotel does not have rooms of a Fake_Room type");
    }

    [Fact]
    public void WhenHotelHaveNoBookingsForRoomTypeInDateRange_ThenShouldReturnRoomCount()
    {
        var result = _sup.Availability("H0", _startDate, _endDate, "RT0");

        result.Should().BeOfType<ErrorOr<int>>();
        result.Value.Should().Be(3);
    }

    [Fact]
    public void WhenHotelHaveBookingsForRoomTypeInDateRange_ThenShouldReturnDifferenceBetweenRoomsAndBookings()
    {
        var result = _sup.Availability("H0", _startDate, _endDate, "RT1");

        result.Should().BeOfType<ErrorOr<int>>();
        result.Value.Should().Be(2);
    }
}