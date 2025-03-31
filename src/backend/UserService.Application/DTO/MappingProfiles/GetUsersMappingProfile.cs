using AutoMapper;
using UserService.Application.DTO.User;

namespace UserService.Application.DTO.MappingProfiles;

public class GetUsersMappingProfile : Profile
{
    public GetUsersMappingProfile()
    {
        CreateMap<Domain.Models.User, UserGetDto>().ReverseMap();
    }
}