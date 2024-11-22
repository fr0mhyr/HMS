using HMS.Application.Common;
using HMS.Domain;
using HMS.Infrastructure.Common;

namespace HMS.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly RepositoryConfiguration _configuration;
    private readonly Lazy<List<Hotel>> _hotels;

    public HotelRepository(RepositoryConfiguration configuration)
    {
        _configuration = configuration;
        _hotels = new Lazy<List<Hotel>>(LoadHotelsFromFile);
    }

    private IReadOnlyList<Hotel> Hotels => _hotels.Value;

    public Hotel? GetById(string id)
    {
        return Hotels.SingleOrDefault(h => h.Id == id);
    }

    private List<Hotel> LoadHotelsFromFile()
    {
        var data = JsonFileReader.Read<List<Hotel>>(_configuration.HotelsFileName);

        if (data == null)
            throw new Exception("No hotel file found or it is empty.");

        return data;
    }
}