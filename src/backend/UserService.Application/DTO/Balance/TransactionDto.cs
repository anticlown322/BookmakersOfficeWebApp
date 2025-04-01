namespace UserService.Application.DTO.Balance;

public record TransactionDto
{
    public string TransactionId { get; init; }
    public decimal Amount { get; init; }
    public string OperationType { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? Comment { get; init; }
    public string Status { get; init; }

    public TransactionDto()
    {
        OperationType = string.Empty;
        Status = string.Empty;
    }
}