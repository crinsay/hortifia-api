namespace Hortifia.Application.Rooms.Dtos;

public class RoomListDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string UserId { get; init; } = default!;
    public List<string> PlantImgUrls { get; set; } = [];
}
