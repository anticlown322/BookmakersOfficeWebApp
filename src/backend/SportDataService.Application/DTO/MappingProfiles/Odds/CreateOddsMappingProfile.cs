using AutoMapper;
using SportDataService.Application.DTO.Odds;
using SportDataService.Domain.Models;

namespace SportDataService.Application.DTO.MappingProfiles.Odds;

public class CreateOddsMappingProfile : Profile
{
    public CreateOddsMappingProfile()
    {
        CreateMap<OddsCreateDto, Domain.Models.Odds>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<OddsValueCreateDto, OddsValue>();
    }
}