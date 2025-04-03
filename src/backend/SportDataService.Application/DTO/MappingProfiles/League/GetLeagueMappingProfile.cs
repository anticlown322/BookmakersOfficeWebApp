using AutoMapper;
using SportDataService.Application.DTO.League;

namespace SportDataService.Application.DTO.MappingProfiles.League;

public class GetLeagueMappingProfile : Profile
{
    public GetLeagueMappingProfile()
    {
        CreateMap<Domain.Models.League, LeagueGetDto>();
    }
}