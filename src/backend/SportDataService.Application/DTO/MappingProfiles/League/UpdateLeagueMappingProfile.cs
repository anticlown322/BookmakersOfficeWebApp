using AutoMapper;
using SportDataService.Application.DTO.League;

namespace SportDataService.Application.DTO.MappingProfiles.League;

public class UpdateLeagueMappingProfile : Profile
{
    public UpdateLeagueMappingProfile()
    {
        CreateMap<LeagueUpdateDto, Domain.Models.League>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
            .ForMember(dest => dest.Country, opt => opt.Condition(src => src.Country != null))
            .ForMember(dest => dest.SportType, opt => opt.Condition(src => src.SportType != null))
            .ForMember(dest => dest.Season, opt => opt.Condition(src => src.Season != null))
            .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue));
    }
}