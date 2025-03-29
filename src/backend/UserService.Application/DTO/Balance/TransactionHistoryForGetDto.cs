namespace UserService.Application.DTO.Balance;

public record TransactionHistoryForGetDto(
    IReadOnlyCollection<TransactionDto> Transactions,
    int TotalCount
);