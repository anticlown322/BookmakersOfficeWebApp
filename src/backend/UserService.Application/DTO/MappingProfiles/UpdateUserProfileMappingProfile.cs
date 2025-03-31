using AutoMapper;
using UserService.Application.DTO.Account;

namespace UserService.Application.DTO.MappingProfiles;

public class UpdateUserProfileMappingProfile : Profile
{
    public UpdateUserProfileMappingProfile()
    {
        CreateMap<UserProfileUpdateDto, Domain.Models.User>()
            .ForPath(dest => dest.Profile.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForPath(dest => dest.Profile.LastName, opt => opt.MapFrom(src => src.LastName));
    }
}