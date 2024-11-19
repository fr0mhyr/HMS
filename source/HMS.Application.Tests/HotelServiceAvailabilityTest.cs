using ErrorOr;
using FluentAssertions;
using HMS.Application.Common;
using HMS.Domain;
using NSubstitute;
using Xunit;

namespace HMS.Application.Tests;

public class HotelServiceAvailabilityTest
{
    private readonly IHotelRepository _hotelRepository = Substitute.For<IHotelRepository>();
    private readonly IBookingRepository _bookingRepository = Substitute.For<IBookingRepository>();

    private readonly DateTime _startDate = new DateTime(2020, 1, 1);
    private readonly DateTime _endDate = new DateTime(2020, 1, 31);

    public HotelServiceAvailabilityTest()
    {
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

                    new Room { RoomType = "RT3" },
                    new Room { RoomType = "RT3" },

                    new Room { RoomType = "RT4" },
                    new Room { RoomType = "RT4" },
                ]
            });
    }

    [Fact]
    public void WhenHotelDoesNotExist_ThenShouldReturnError()
    {
        var sup = new HotelService(_hotelRepository, _bookingRepository);

        var result = sup.Availability("Fake_Hotel", _startDate, _endDate, "Fake_Room");

        result.Should().BeOfType<ErrorOr<int>>();
        result.Errors.First().Code.Should().Be("No hotel found with id Fake_Hotel");
    }

    [Fact]
    public void WhenHotelDoesNotHaveSpecificRoomType_ThenShouldReturnError()
    {
        var sup = new HotelService(_hotelRepository, _bookingRepository);

        var result = sup.Availability("H0", _startDate, _endDate, "Fake_Room");

        result.Should().BeOfType<ErrorOr<int>>();
        result.Errors.First().Code.Should().Be("The hotel does not have rooms of a Fake_Room type");
    }

    [Fact]
    public void WhenHotelHaveNoBookings_ThenShouldReturnRoomCount()
    {
        _bookingRepository.GetBookings("H0", "RT0", Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new List<Booking>());
        var sup = new HotelService(_hotelRepository, _bookingRepository);

        var result = sup.Availability("H0", _startDate, _endDate, "RT0");

        result.Should().BeOfType<ErrorOr<int>>();
        result.Value.Should().Be(3);
    }

    [Fact]
    public void WhenHotelHaveAllOverlappingBookings_ThenShouldReturnDifferenceBetweenRoomsAndBookings()
    {
        _bookingRepository.GetBookings("H0", "RT1", _startDate, _endDate)
            .Returns(new List<Booking>()
            {
                new Booking()
                {
                    Arrival = new DateTime(2019, 12, 30),
                    Departure = new DateTime(2020, 1, 15),
                },
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 1),
                    Departure = new DateTime(2020, 1, 14),
                },
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 10),
                    Departure = new DateTime(2020, 1, 20),
                },
                new Booking()
                {
                    Arrival = new DateTime(2019, 12, 1),
                    Departure = new DateTime(2020, 2, 29),
                },
            });
        var sup = new HotelService(_hotelRepository, _bookingRepository);

        var result = sup.Availability("H0", _startDate, _endDate, "RT1");

        result.Value.Should().Be(2);
    }

    [Fact]
    public void WhenHotelHaveMoreOverlappingBookingsThanRooms_ShouldReturnNegativeNumber()
    {
        _bookingRepository.GetBookings("H0", "RT2", _startDate, _endDate)
            .Returns(new List<Booking>()
            {
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 1),
                    Departure = new DateTime(2020, 1, 10),
                },
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 2),
                    Departure = new DateTime(2020, 1, 20),
                },
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 3),
                    Departure = new DateTime(2020, 1, 31),
                },
            });
        var sup = new HotelService(_hotelRepository, _bookingRepository);

        var result = sup.Availability("H0", _startDate, _endDate, "RT2");

        result.Value.Should().Be(-1);
    }

    [Fact]
    public void WhenHotelHaveAllNonOverlappingBookings_ThenShouldUseOneRoomOnly()
    {
        _bookingRepository.GetBookings("H0", "RT3", _startDate, _endDate)
            .Returns(new List<Booking>()
            {
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 1),
                    Departure = new DateTime(2020, 1, 10),
                },
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 11),
                    Departure = new DateTime(2020, 1, 20),
                },
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 21),
                    Departure = new DateTime(2020, 1, 25),
                },
                new Booking()
                {
                    Arrival = new DateTime(2020, 1, 26),
                    Departure = new DateTime(2020, 1, 31),
                },
            });
        var sup = new HotelService(_hotelRepository, _bookingRepository);

        var result = sup.Availability("H0", _startDate, _endDate, "RT3");

        result.Value.Should().Be(1);
    }

    [Fact]
    public void WhenBookingStartBeforeStartDateAndEndAfterEndDate_ThenShouldUseOneRoomOnly()
    {
        _bookingRepository.GetBookings("H0", "RT4", _startDate, _endDate)
            .Returns(new List<Booking>()
            {
                new Booking()
                {
                    Arrival = new DateTime(2019, 12, 1),
                    Departure = new DateTime(2020, 2, 10),
                },
            });
        var sup = new HotelService(_hotelRepository, _bookingRepository);
        
        var result = sup.Availability("H0", _startDate, _endDate, "RT4");

        result.Value.Should().Be(1);
    }
}