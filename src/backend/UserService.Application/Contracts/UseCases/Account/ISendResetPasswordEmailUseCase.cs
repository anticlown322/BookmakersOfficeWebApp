namespace UserService.Application.Contracts.UseCases.Account;

public interface ISendResetPasswordEmailUseCase
{
    public Task ExecuteAsync(string username, CancellationToken cancellationToken);
}