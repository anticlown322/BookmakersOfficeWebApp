using AutoMapper;
using SportDataService.Application.DTO.Player;

namespace SportDataService.Application.DTO.MappingProfiles.Player;

public class GetPlayerMappingProfile : Profile
{
    public GetPlayerMappingProfile()
    {
        CreateMap<Domain.Models.Player, PlayerGetDto>().ReverseMap();
    }
}