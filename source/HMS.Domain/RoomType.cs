namespace HMS.Domain;

public class RoomType
{
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<string> Amenities { get; set; } = null!;
    public List<string> Features { get; set; } = null!;
}