using AutoMapper;
using SportDataService.Application.DTO.Lines;
using SportDataService.Application.DTO.Match;
using SportDataService.Application.DTO.Team;
using SportDataService.Domain.Models.Lines;
using SportDataService.Domain.Models.Markets;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetMatchMappingProfile : Profile
{
    public GetMatchMappingProfile()
    {
        CreateMap<MarketValue, MarketValueGetDto>();

        CreateMap<MainLine, MainLineGetDto>()
            .ForMember(dest => dest.Opponent1Win, opt => opt.MapFrom(src => src.Opponent1Win))
            .ForMember(dest => dest.Opponent2Win, opt => opt.MapFrom(src => src.Opponent2Win))
            .ForMember(dest => dest.Draw, opt => opt.MapFrom(src => src.Draw))
            .ForMember(dest => dest.Opponent1WinOrDraw, opt => opt.MapFrom(src => src.Opponent1WinOrDraw))
            .ForMember(dest => dest.Opponent2WinOrDraw, opt => opt.MapFrom(src => src.Opponent2WinOrDraw));

        CreateMap<KillsLine, KillsLineGetDto>()
            .ForMember(dest => dest.Opponent1KillsMain, opt => opt.MapFrom(src => src.Opponent1KillsMain))
            .ForMember(dest => dest.Opponent2KillsMain, opt => opt.MapFrom(src => src.Opponent2KillsMain))
            .ForMember(dest => dest.TotalKillsUnder, opt => opt.MapFrom(src => src.TotalKillsUnder))
            .ForMember(dest => dest.TotalKillsOver, opt => opt.MapFrom(src => src.TotalKillsOver))
            .ForMember(dest => dest.Opponent1KillsHandicap, opt => opt.MapFrom(src => src.Opponent1KillsHandicap))
            .ForMember(dest => dest.Opponent2KillsHandicap, opt => opt.MapFrom(src => src.Opponent2KillsHandicap));

        CreateMap<MapsLine, MapsLineGetDto>()
            .ForMember(dest => dest.Map1HandicapOpponent1, opt => opt.MapFrom(src => src.Map1HandicapOpponent1))
            .ForMember(dest => dest.Map1HandicapOpponent2, opt => opt.MapFrom(src => src.Map1HandicapOpponent2))
            .ForMember(dest => dest.Map2HandicapOpponent1, opt => opt.MapFrom(src => src.Map2HandicapOpponent1))
            .ForMember(dest => dest.Map2HandicapOpponent2, opt => opt.MapFrom(src => src.Map2HandicapOpponent2));

        CreateMap<SpecialLine, SpecialLineGetDto>()
            .ForMember(dest => dest.EitherOpponent1OrOpponent2, opt => opt.MapFrom(src => src.EitherOpponent1OrOpponent2));

        CreateMap<Domain.Models.Tournaments.Match, MatchGetDto>()
            .ForMember(dest => dest.MainLine, opt => opt.MapFrom(src => src.MainLine))
            .ForMember(dest => dest.KillsLine, opt => opt.MapFrom(src => src.KillsLine))
            .ForMember(dest => dest.MapsLine, opt => opt.MapFrom(src => src.MapsLine))
            .ForMember(dest => dest.SpecialLine, opt => opt.MapFrom(src => src.SpecialLine));
    }
}