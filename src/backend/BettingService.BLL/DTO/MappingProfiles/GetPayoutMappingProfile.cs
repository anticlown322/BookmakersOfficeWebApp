using AutoMapper;
using BettingService.BLL.DTO.Payout;

namespace BettingService.BLL.DTO.MappingProfiles;

public class GetPayoutMappingProfile : Profile
{
    public GetPayoutMappingProfile()
    {
        CreateMap<DAL.Models.Entities.Payout, GetPayoutDto>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); ;
    }
}