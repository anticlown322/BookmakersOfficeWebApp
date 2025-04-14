using AutoMapper;
using SportDataService.Application.DTO.Team;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetTeamMappingProfile : Profile
{
    public GetTeamMappingProfile()
    {
        CreateMap<Domain.Models.Tournaments.Team, TeamGetDto>();
    }
}