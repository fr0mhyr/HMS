using ErrorOr;
using HMS.Application.Services;

namespace HMS.Client.Queries;

public class SearchQuery : IQuery
{
    private const string DateFormat = "yyyyMMdd";

    private readonly IHotelService _hotelService;

    public SearchQuery(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public ErrorOr<string> Execute(string input)
    {
        var parameters = ParseInput(input);

        if (parameters.IsError)
            return Error.Validation(parameters.FirstError.Code);

        var p = parameters.Value;
        var result = _hotelService.Search(p.HotelId, p.RoomType, p.Days);

        if (result.IsError)
            return result.Errors;

        if (result.Value.Count == 0)
            return Environment.NewLine;

        return string.Join(", ",
            result.Value.Select(r =>
                $"({r.StartDate.ToString(DateFormat)}-{r.EndDate.ToString(DateFormat)}, {r.RoomCount})"));
    }

    private ErrorOr<SearchQueryParameters> ParseInput(string input)
    {
        try
        {
            var parameters = input.Substring(input.IndexOf('(') + 1).TrimEnd(')').Split(',');

            var hotelId = parameters[0].Trim();
            var days = int.Parse(parameters[1].Trim());
            var roomType = parameters[2].Trim();

            return new SearchQueryParameters(hotelId, days, roomType);
        }
        catch
        {
            return Error.Validation("Invalid arguments. Should be Search(hotelId,days,roomType)");
        }
    }

    private record SearchQueryParameters(string HotelId, int Days, string RoomType);
}