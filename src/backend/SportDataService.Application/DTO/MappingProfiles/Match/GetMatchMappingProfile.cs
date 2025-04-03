using AutoMapper;
using SportDataService.Application.DTO.Match;
using SportDataService.Domain.Models;

namespace SportDataService.Application.DTO.MappingProfiles.Match;

public class GetMatchMappingProfile : Profile
{
    public GetMatchMappingProfile()
    {
        CreateMap<Domain.Models.Match, MatchGetDto>();
        CreateMap<Score, ScoreGetDto>();
    }
}