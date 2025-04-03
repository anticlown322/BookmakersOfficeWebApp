using AutoMapper;
using SportDataService.Application.DTO.Player;

namespace SportDataService.Application.DTO.MappingProfiles.Player;

public class UpdatePlayerMappingProfile : Profile
{
    public UpdatePlayerMappingProfile()
    {
        CreateMap<PlayerUpdateDto, Domain.Models.Player>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
            .ForMember(dest => dest.TeamId, opt =>
            {
                opt.PreCondition(src => src.TeamId != null);
                opt.MapFrom(src => src.TeamId!.Trim());
            })
            .ForMember(dest => dest.Position, opt =>
            {
                opt.PreCondition(src => src.Position != null); 
                opt.MapFrom(src => src.Position!.Trim());
            });
    }
}