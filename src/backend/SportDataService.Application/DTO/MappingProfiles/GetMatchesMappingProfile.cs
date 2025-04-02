using AutoMapper;
using SportDataService.Application.DTO.Match;
using SportDataService.Domain.Models;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetMatchesMappingProfile : Profile
{
    public GetMatchesMappingProfile()
    {
        CreateMap<Domain.Models.Match, MatchGetDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))

            .ForMember(dest => dest.CurrentScore, opt => opt.MapFrom(src =>
                new ScoreDto(
                    src.CurrentScore.Home,
                    src.CurrentScore.Away)));

        // .ForMember(dest => dest.HomeTeam, opt => opt.MapFrom((src, dest, _, context) => 
        //     context.Mapper.Map<TeamShortDto>(
        //         context.Items["teamsCache"]?[src.HomeTeamId])))
        //
        // .ForMember(dest => dest.AwayTeam, opt => opt.MapFrom((src, dest, _, context) => 
        //     context.Mapper.Map<TeamShortDto>(
        //         context.Items["teamsCache"]?[src.AwayTeamId])))
        //
        // .ForMember(dest => dest.Events, opt => opt.MapFrom((src, dest, _, context) =>
        //     context.Mapper.Map<IEnumerable<EventShortDto>>(
        //         context.Items["eventsCache"]?[src.Id] ?? Enumerable.Empty<Event>())));
    }
}