# HMS
Hotel Management System

### Build and Run

```sh
dotnet build
```

```sh
dotnet run --project ./source/HMS.Client --hotels hotels.json --bookings bookings.json
```

### Actions
- Enter - close application
- Search - Search(HotelId, Days, RoomType)
- Availability - Availability(HotelId, DateOrRange, RoomType)

DateOrRange examples:
- 20250101
- 20250101-20250201