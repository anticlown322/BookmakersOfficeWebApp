using AutoMapper;
using UserService.Application.DTO.Account;

namespace UserService.Application.DTO.MappingProfiles;

public class GetUserProfileMappingProfile : Profile
{
    public GetUserProfileMappingProfile()
    {
        CreateMap<Domain.Models.User, UserProfileGetDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Profile.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Profile.LastName))
            .ForMember(
                dest => dest.Roles,
                opt => opt.MapFrom((src, dest, _, context) =>
                    context.Items["Roles"] as List<string> ?? new List<string>()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.isEmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));
    }
}