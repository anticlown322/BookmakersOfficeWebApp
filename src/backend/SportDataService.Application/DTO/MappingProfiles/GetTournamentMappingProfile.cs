using AutoMapper;
using SportDataService.Application.DTO.Tournament;

namespace SportDataService.Application.DTO.MappingProfiles;

public class GetTournamentMappingProfile : Profile
{
    public GetTournamentMappingProfile()
    {
        CreateMap<Domain.Models.Tournaments.Tournament, TournamentGetDto>()
            .ForMember(dest => dest.Matches, opt => opt.MapFrom(src => src.Matches));
    }
}