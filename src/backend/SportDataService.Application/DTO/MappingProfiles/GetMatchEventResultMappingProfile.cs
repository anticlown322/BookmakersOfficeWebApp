using AutoMapper;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.Models.Results;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetMatchEventResultMappingProfile : Profile
{
    public GetMatchEventResultMappingProfile()
    {
        CreateMap<MatchEventResult, MatchEventResultGetDto>()
            .ForMember(
                dest => dest.SubScores,
                opt =>
                {
                    opt.Condition(src => src.SubScores != null && src.SubScores.Any());
                    opt.MapFrom(src => src.SubScores);
                });
    }
}