using ErrorOr;
using FluentAssertions;
using HMS.Application.Common;
using HMS.Domain;
using NSubstitute;
using Xunit;

namespace HMS.Application.Tests;

public class HotelServiceSearchTests
{
    private readonly DateTime _now = new DateTime(2020, 1, 1);

    private readonly HotelService _sup;

    private readonly IHotelRepository _hotelRepository = Substitute.For<IHotelRepository>();
    private readonly IBookingRepository _bookingRepository = Substitute.For<IBookingRepository>();
    private readonly IDateService _dateService = Substitute.For<IDateService>();

    public HotelServiceSearchTests()
    {
        _hotelRepository.GetById("H0")
            .Returns(new Hotel()
            {
                Id = "H0",
                Rooms =
                [
                    new Room { RoomType = "RT0" },
                    new Room { RoomType = "RT0" },
                    new Room { RoomType = "RT0" },
                ]
            });
        _dateService.NowDate.Returns(_now);

        _sup = new HotelService(_hotelRepository, _bookingRepository, _dateService);
    }

    [Fact]
    public void WhenHotelRoomTypeHaveNoBookings_ShouldReturnOneResult()
    {
        var result = _sup.Search("H0", "RT0", 10);

        var expected = new RoomTypeAvailabilityRange(new DateTime(2020, 1,1), new DateTime(2020, 1,10), 3);

        result.Should().BeOfType<ErrorOr<IList<RoomTypeAvailabilityRange>>>();
        result.Value.First().Should().BeEquivalentTo(expected, options => options
            .ComparingRecordsByValue()
            .ComparingByMembers<RoomTypeAvailabilityRange>());
    }

    [Fact]
    public void WhenHotelHaveEqualOrMoreOverlappingBookingsThanRooms_ShouldReturnEmptyResult()
    {
        _bookingRepository.GetBookings("H0", "RT0", Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new List<Booking>
            {
                new Booking
                {
                    Arrival = new DateTime(2020, 1, 1),
                    Departure = new DateTime(2020, 1, 10),
                },
                new Booking
                {
                    Arrival = new DateTime(2020, 1, 1),
                    Departure = new DateTime(2020, 1, 10),
                },
                new Booking
                {
                    Arrival = new DateTime(2020, 1, 1),
                    Departure = new DateTime(2020, 1, 6),
                },
                new Booking
                {
                    Arrival = new DateTime(2020, 1, 5),
                    Departure = new DateTime(2020, 1, 10),
                },
            });

        var result = _sup.Search("H0", "RT0", 10);

        var expected = new RoomTypeAvailabilityRange(_now, _now.AddDays(9), 3);

        result.Should().BeOfType<ErrorOr<IList<RoomTypeAvailabilityRange>>>();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public void WhenHotelHaveManyBookings_ThenShouldReturnCorrectResult()
    {
        _bookingRepository.GetBookings("H0", "RT0", Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(new List<Booking>
            {
                new Booking
                {
                    Arrival = new DateTime(2019, 12, 25),
                    Departure = new DateTime(2020, 1, 10),
                },
                new Booking
                {
                    Arrival = new DateTime(2020, 1, 8),
                    Departure = new DateTime(2020, 2, 1),
                },
                new Booking
                {
                    Arrival = new DateTime(2020, 1, 3),
                    Departure = new DateTime(2020, 1, 9),
                },
                new Booking
                {
                    Arrival = new DateTime(2020, 1, 8),
                    Departure = new DateTime(2020, 2, 2),
                },
                new Booking
                {
                    Arrival = new DateTime(2020, 1, 7),
                    Departure = new DateTime(2020, 1, 13),
                },
            });

        // start:   1-1-2020
        // end:     5-2-2020 (+36)
        var result = _sup.Search("H0", "RT0", 36);

        var expected = new List<RoomTypeAvailabilityRange>
        {
            new RoomTypeAvailabilityRange(new DateTime(2020, 1, 1), new DateTime(2020, 1, 2), 2),
            new RoomTypeAvailabilityRange(new DateTime(2020, 1, 3), new DateTime(2020, 1, 6), 1),
            new RoomTypeAvailabilityRange(new DateTime(2020, 1, 14), new DateTime(2020, 2, 1), 1),
            new RoomTypeAvailabilityRange(new DateTime(2020, 2, 2), new DateTime(2020, 2, 2), 2),
            new RoomTypeAvailabilityRange(new DateTime(2020, 2, 3), new DateTime(2020, 2, 5), 3),
        };

        result.Should().BeOfType<ErrorOr<IList<RoomTypeAvailabilityRange>>>();
        result.Value.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}