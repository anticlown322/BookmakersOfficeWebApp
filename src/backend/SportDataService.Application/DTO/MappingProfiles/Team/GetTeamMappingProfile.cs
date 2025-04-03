using AutoMapper;
using SportDataService.Application.DTO.Team;

namespace SportDataService.Application.DTO.MappingProfiles.Team;

public class GetTeamMappingProfile : Profile
{
    public GetTeamMappingProfile()
    {
        CreateMap<Domain.Models.Team, TeamGetDto>();
    }
}