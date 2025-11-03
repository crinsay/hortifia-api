using AutoMapper;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Rooms.Dtos;

internal class RoomMappingProfile : Profile
{
    public RoomMappingProfile()
    {
        CreateMap<Room, RoomDto>();
    }
}
