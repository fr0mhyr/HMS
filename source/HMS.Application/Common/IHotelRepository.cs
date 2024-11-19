using HMS.Domain;

namespace HMS.Application.Common;

public interface IHotelRepository
{
    Hotel? GetById(string id);
}