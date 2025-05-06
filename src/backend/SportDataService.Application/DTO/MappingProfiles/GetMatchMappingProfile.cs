using AutoMapper;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Prematch.Lines;
using SportDataService.Domain.Models.Prematch.Lines;
using SportDataService.Domain.Models.Prematch.Markets;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetMatchMappingProfile : Profile
{
    public GetMatchMappingProfile()
    {
        CreateMap<MarketValue, MarketValueGetDto>()
            .ForMember(dest => dest.Value, opt => opt.Condition(src => src.Value != null));

        CreateMap<MainLine, MainLineGetDto>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null && !IsEmptyMarketValue(srcMember)));

        CreateMap<KillsLine, KillsLineGetDto>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null && !IsEmptyMarketValue(srcMember)));

        CreateMap<MapsLine, MapsLineGetDto>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null && !IsEmptyMarketValue(srcMember)));

        CreateMap<SpecialLine, SpecialLineGetDto>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
                srcMember != null && !IsEmptyMarketValue(srcMember)));

        CreateMap<Domain.Models.Prematch.Match, MatchGetDto>()
            .ForMember(
                dest => dest.MainLine,
                opt => opt.MapFrom(src => !IsEmptyLine(src.MainLine) ? src.MainLine : null))
            .ForMember(
                dest => dest.KillsLine,
                opt => opt.MapFrom(src => !IsEmptyLine(src.KillsLine) ? src.KillsLine : null))
            .ForMember(
                dest => dest.MapsLine,
                opt => opt.MapFrom(src => !IsEmptyLine(src.MapsLine) ? src.MapsLine : null))
            .ForMember(
                dest => dest.SpecialLine,
                opt => opt.MapFrom(src => !IsEmptyLine(src.SpecialLine) ? src.SpecialLine : null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }

    private bool IsEmptyMarketValue(object? value)
    {
        return value is MarketValue { Value: null, UpdatedAt: null, IsActive: false };
    }

    private bool IsEmptyLine(object? line)
    {
        return line switch
        {
            MainLine mainLine => IsEmptyMainLine(mainLine),
            KillsLine killsLine => IsEmptyKillsLine(killsLine),
            MapsLine mapsLine => IsEmptyMapsLine(mapsLine),
            SpecialLine specialLine => IsEmptySpecialLine(specialLine),
            _ => true
        };
    }

    private bool IsEmptyMainLine(MainLine line)
    {
        return line is
        {
            Opponent1Win: null,
            Opponent2Win: null,
            Draw: null,
            Opponent1WinOrDraw: null,
            Opponent2WinOrDraw: null
        };
    }

    private bool IsEmptyKillsLine(KillsLine line)
    {
        return line is
        {
            Opponent1KillsMain: null,
            Opponent2KillsMain: null,
            TotalKillsUnder: null,
            TotalKillsOver: null,
            Opponent1KillsHandicap: null,
            Opponent2KillsHandicap: null
        };
    }

    private bool IsEmptyMapsLine(MapsLine line)
    {
        return line is
        {
            Map1HandicapOpponent1: null,
            Map1HandicapOpponent2: null,
            Map2HandicapOpponent1: null,
            Map2HandicapOpponent2: null
        };
    }

    private bool IsEmptySpecialLine(SpecialLine line)
    {
        return line is { EitherOpponent1OrOpponent2: null };
    }
}