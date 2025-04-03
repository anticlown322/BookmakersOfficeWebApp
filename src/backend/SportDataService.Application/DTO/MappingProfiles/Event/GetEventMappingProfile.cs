using AutoMapper;
using SportDataService.Application.DTO.Event;

namespace SportDataService.Application.DTO.MappingProfiles.Event;

public class GetEventMappingProfile : Profile
{
    public GetEventMappingProfile()
    {
        CreateMap<Domain.Models.Event, EventGetDto>();
    }
}