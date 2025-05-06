using AutoMapper;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.Models.Results;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetTournamentResultMappingProfile : Profile
{
    public GetTournamentResultMappingProfile()
    {
        CreateMap<TournamentResult, TournamentResultGetDto>()
            .ForMember(
                dest => dest.Matches,
                opt =>
                {
                    opt.MapFrom(src => src.Matches);
                    opt.Condition(src => src.Matches != null && src.Matches.Any());
                });
    }
}