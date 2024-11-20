using HMS.Domain;

namespace HMS.Application.Extensions;

public static class BookingListExtensions
{
    public static int MinimumRoomsDemand(this IList<Booking> bookings)
    {
        IList<(DateTime date, int change)> events = new List<(DateTime date, int change)>();

        foreach (var booking in bookings)
        {
            events.Add((booking.Arrival, 1));
            events.Add((booking.Departure.AddDays(1), -1));
        }

        events = events
            .OrderBy(e => e.date)
            .ThenBy(e => e.change)
            .ToList();

        var max = 0;
        var current = 0;

        foreach (var e in events)
        {
            current += e.change;
            max = Math.Max(current, max);
        }

        return max;
    }
}