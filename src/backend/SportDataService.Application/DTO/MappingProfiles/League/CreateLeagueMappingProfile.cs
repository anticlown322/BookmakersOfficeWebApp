using AutoMapper;
using SportDataService.Application.DTO.League;

namespace SportDataService.Application.DTO.MappingProfiles.League;

public class CreateLeagueMappingProfile : Profile
{
    public CreateLeagueMappingProfile()
    {
        CreateMap<LeagueCreateDto, Domain.Models.League>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Trim()));
    }
}