using AutoMapper;
using SportDataService.Application.DTO.Event;

namespace SportDataService.Application.DTO.MappingProfiles.Event;

public class CreateEventMappingProfile : Profile
{
    public CreateEventMappingProfile()
    {
        CreateMap<EventCreateDto, Domain.Models.Event>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}