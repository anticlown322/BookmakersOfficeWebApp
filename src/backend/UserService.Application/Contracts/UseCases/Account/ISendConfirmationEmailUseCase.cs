namespace UserService.Application.Contracts.UseCases.Account;

public interface ISendConfirmationEmailUseCase
{
    public Task ExecuteAsync(string username, CancellationToken cancellationToken);
}