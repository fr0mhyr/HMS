namespace HMS.Domain;

public class Hotel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public List<RoomType> RoomTypes { get; set; } = null!;
    public List<Room> Rooms { get; set; } = null!;
}