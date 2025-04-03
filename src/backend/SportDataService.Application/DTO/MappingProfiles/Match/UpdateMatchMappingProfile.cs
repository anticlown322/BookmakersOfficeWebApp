using AutoMapper;
using SportDataService.Application.DTO.Match;
using SportDataService.Domain.Models;

namespace SportDataService.Application.DTO.MappingProfiles.Match;

public class UpdateMatchMappingProfile : Profile
{
    public UpdateMatchMappingProfile()
    {
        CreateMap<MatchUpdateDto, Domain.Models.Match>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentScore, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<ScoreUpdateDto, Score>()
            .ForMember(dest => dest.Home, opt => opt.Condition(src => src.Home != null))
            .ForMember(dest => dest.Away, opt => opt.Condition(src => src.Away != null));
    }
}