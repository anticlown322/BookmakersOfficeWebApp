using AutoMapper;
using SportDataService.Application.DTO.Common;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetTeamMappingProfile : Profile
{
    public GetTeamMappingProfile()
    {
        CreateMap<Domain.Models.Common.Team, TeamGetDto>();
    }
}