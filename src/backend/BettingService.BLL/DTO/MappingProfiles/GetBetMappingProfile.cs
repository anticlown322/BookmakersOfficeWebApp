using AutoMapper;
using BettingService.BLL.DTO.Bet;

namespace BettingService.BLL.DTO.MappingProfiles;

public class GetBetMappingProfile : Profile
{
    public GetBetMappingProfile()
    {
        CreateMap<DAL.Models.Entities.Bet, GetBetDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.MatchId, opt => opt.MapFrom(src => src.MatchId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Odds, opt => opt.MapFrom(src => src.Odds))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.SettledAt, opt => opt.MapFrom(src => src.SettledAt));
    }
}