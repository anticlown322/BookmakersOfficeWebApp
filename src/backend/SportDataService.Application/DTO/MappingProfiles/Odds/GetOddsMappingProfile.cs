using AutoMapper;
using SportDataService.Application.DTO.Odds;
using SportDataService.Domain.Models;

namespace SportDataService.Application.DTO.MappingProfiles.Odds;

public class GetOddsMappingProfile : Profile
{
    public GetOddsMappingProfile()
    {
        CreateMap<Domain.Models.Odds, OddsGetDto>();
        CreateMap<OddsValue, OddsValueGetDto>();
    }
}