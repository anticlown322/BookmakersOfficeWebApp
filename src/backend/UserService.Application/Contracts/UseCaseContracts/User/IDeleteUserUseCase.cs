namespace UserService.Application.Contracts.UseCaseContracts.User;

public interface IDeleteUserUseCase
{
    Task ExecuteAsync(string userName, CancellationToken cancellationToken);
}