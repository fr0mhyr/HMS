namespace HMS.Domain;

public class Booking
{
    public string HotelId { get; set; } = null!;
    public DateTime Arrival { get; set; }
    public DateTime Departure { get; set; }
    public string RoomType { get; set; } = null!;
    public string RoomRate { get; set; } = null!;
}