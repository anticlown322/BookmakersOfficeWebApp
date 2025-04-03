using AutoMapper;
using SportDataService.Application.DTO.Match;
using SportDataService.Domain.Models;

namespace SportDataService.Application.DTO.MappingProfiles.Match;

public class CreateMatchMappingProfile : Profile
{
    public CreateMatchMappingProfile()
    {
        CreateMap<MatchCreateDto, Domain.Models.Match>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(
                dest => dest.CurrentScore,
                opt => opt.MapFrom(src => src.InitialScore ?? new ScoreCreateDto()))
            .ForMember(dest => dest.EventIds, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<ScoreCreateDto, Score>();
    }
}