using AutoMapper;
using UserService.Application.DTO.Authentication;
using UserService.Domain.Models;

namespace UserService.Application.DTO.MappingProfiles;

public class RegisterUserMappingProfile : Profile
{
    public RegisterUserMappingProfile()
    {
        CreateMap<UserRegistrationDto, Domain.Models.User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Profile, opt => opt.MapFrom(src => new UserProfile
            {
                FirstName = src.FirstName,
                LastName = src.LastName,
                UserId = "TempValue" 
            }));
    }
}