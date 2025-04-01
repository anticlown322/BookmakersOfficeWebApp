namespace UserService.Application.Contracts.UseCases.Account;

public interface IConfirmEmailUseCase
{
    public Task ExecuteAsync(string username, CancellationToken cancellationToken);
}