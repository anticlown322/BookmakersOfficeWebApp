using AutoMapper;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.Models.Results;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetMatchResultMappingProfile : Profile
{
    public GetMatchResultMappingProfile()
    {
        CreateMap<MatchResult, MatchResultGetDto>()
            .ForMember(
                dest => dest.SubScores,
                opt =>
                {
                    opt.Condition(src => src.SubScores != null && src.SubScores.Any());
                    opt.MapFrom(src => src.SubScores);
                })
            .ForMember(
                dest => dest.EventResults,
                opt =>
                {
                    opt.Condition(src => src.EventResults != null && src.EventResults.Any());
                    opt.MapFrom(src => src.EventResults);
                });
    }
}