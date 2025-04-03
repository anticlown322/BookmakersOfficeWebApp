using AutoMapper;
using SportDataService.Application.DTO.Team;

namespace SportDataService.Application.DTO.MappingProfiles.Team;

public class UpdateTeamMappingProfile : Profile
{
    public UpdateTeamMappingProfile()
    {
        CreateMap<TeamUpdateDto, Domain.Models.Team>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
            .ForMember(dest => dest.ShortName, opt => opt.Condition(src => src.ShortName != null))
            .ForMember(dest => dest.Country, opt => opt.Condition(src => src.Country != null))
            .ForMember(dest => dest.SportType, opt => opt.Condition(src => src.SportType != null))
            .ForMember(dest => dest.PlayerIds, opt => opt.Ignore());
    }
}