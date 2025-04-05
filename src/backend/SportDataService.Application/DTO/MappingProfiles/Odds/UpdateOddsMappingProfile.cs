using AutoMapper;
using SportDataService.Application.DTO.Odds;
using SportDataService.Domain.Models;

namespace SportDataService.Application.DTO.MappingProfiles.Odds;

public class UpdateOddsMappingProfile : Profile
{
    public UpdateOddsMappingProfile()
    {
        CreateMap<OddsUpdateDto, Domain.Models.Odds>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.MatchId, opt => opt.Ignore())
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<OddsValueUpdateDto, OddsValue>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

    }
}