using AutoMapper;
using UserService.Application.DTO.Balance;
using UserService.Application.Utility;
using UserService.Domain.Models;

namespace UserService.Application.DTO.MappingProfiles;

public class GetTransactionsMappingProfile : Profile
{
    public GetTransactionsMappingProfile()
    {
        CreateMap<BalanceTransaction, TransactionDto>()
            .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.OperationType, opt => opt.MapFrom(src => src.OperationType))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BalanceOperationTypesAndStatuses.CompletedStatus));
    }
}