using AutoMapper;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.DTO.MappingProfiles;

public class RegisterUserMappingProfile : Profile
{
    public RegisterUserMappingProfile()
    {
        CreateMap<UserForRegistrationDto, Domain.Models.User>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
    }
}