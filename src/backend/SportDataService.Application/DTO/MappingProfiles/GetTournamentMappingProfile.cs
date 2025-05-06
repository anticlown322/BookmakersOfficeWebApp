using AutoMapper;
using SportDataService.Application.DTO.Prematch;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetTournamentMappingProfile : Profile
{
    public GetTournamentMappingProfile()
    {
        CreateMap<Domain.Models.Prematch.Tournament, TournamentGetDto>()
            .ForMember(dest => dest.Matches, opt => opt.MapFrom(src => src.Matches));
    }
}