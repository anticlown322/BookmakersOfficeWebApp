using AutoMapper;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.Models.Results;

namespace SportDataService.Application.DTO.MappingProfiles;


public class GetSubScoreMappingProfile : Profile
{
    public GetSubScoreMappingProfile()
    {
        CreateMap<SubScore, SubScoreGetDto>();
    }
}
