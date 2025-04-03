using AutoMapper;
using SportDataService.Application.DTO.Event;

namespace SportDataService.Application.DTO.MappingProfiles.Event;

public class UpdateEventMappingProfile : Profile
{
    public UpdateEventMappingProfile()
    {
        CreateMap<EventUpdateDto, Domain.Models.Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.MatchId, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}