using ErrorOr;
using HMS.Application.Services;

namespace HMS.Client.Queries;

public class AvailabilityQuery : IQuery
{
    private const string DateFormat = "yyyyMMdd";

    private readonly IHotelService _hotelService;

    public AvailabilityQuery(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public ErrorOr<string> Execute(string input)
    {
        var parameters = ParseInput(input);

        if (parameters.IsError)
            return Error.Validation(parameters.FirstError.Code);

        var p = parameters.Value;
        var result = _hotelService.Availability(p.HotelId, p.StartDate, p.EndDate, p.RoomType);

        if (result.IsError)
            return result.Errors;

        return result.Value.ToString();
    }

    private ErrorOr<AvailabilityQueryParameters> ParseInput(string input)
    {
        try
        {
            var parameters = input.Substring(input.IndexOf('(') + 1).TrimEnd(')').Split(',');

            var hotelId = parameters[0].Trim();
            var dateOrRange = parameters[1].Trim();
            var roomType = parameters[2].Trim();

            DateTime startDate, endDate;

            if (dateOrRange.Contains("-"))
            {
                var dates = dateOrRange.Split('-');
                startDate = DateTime.ParseExact(dates[0].Trim(), DateFormat, null);
                endDate = DateTime.ParseExact(dates[1].Trim(), DateFormat, null);
            }
            else
            {
                startDate = DateTime.ParseExact(dateOrRange, DateFormat, null);
                endDate = startDate;
            }

            return new AvailabilityQueryParameters(hotelId, startDate, endDate, roomType);
        }
        catch
        {
            return Error.Validation("Invalid arguments. Should be Availability(hotelId,dateOrRange,roomType)");
        }
    }

    private record AvailabilityQueryParameters(string HotelId, DateTime StartDate, DateTime EndDate, string RoomType);
}