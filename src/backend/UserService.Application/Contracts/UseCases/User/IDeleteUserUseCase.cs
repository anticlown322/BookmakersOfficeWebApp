namespace UserService.Application.Contracts.UseCases.User;

public interface IDeleteUserUseCase
{
    Task ExecuteAsync(string userName, CancellationToken cancellationToken);
}