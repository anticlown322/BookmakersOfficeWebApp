using AutoMapper;
using SportDataService.Application.DTO.Player;

namespace SportDataService.Application.DTO.MappingProfiles.Player;

public class CreatePlayerMappingProfile : Profile
{
    public CreatePlayerMappingProfile()
    {
        CreateMap<PlayerCreateDto, Domain.Models.Player>()
        .ForMember(dest => dest.Id, opt => opt.Ignore())
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
        .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.TeamId) ? src.TeamId.Trim() : null));
    }
}