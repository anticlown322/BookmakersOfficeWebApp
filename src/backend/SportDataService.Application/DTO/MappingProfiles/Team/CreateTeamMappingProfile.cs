using AutoMapper;
using SportDataService.Application.DTO.Team;

namespace SportDataService.Application.DTO.MappingProfiles.Team;

public class CreateTeamMappingProfile : Profile
{
    public CreateTeamMappingProfile()
    {
        CreateMap<TeamCreateDto, Domain.Models.Team>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PlayerIds, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
            .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.ShortName.Trim().ToUpper()));
    }
}