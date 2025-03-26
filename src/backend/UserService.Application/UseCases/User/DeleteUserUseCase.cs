using UserService.Application.Contracts.UseCases.User;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.User;

public class DeleteUserUseCase(IUsersRepository usersRepository) : IDeleteUserUseCase
{
    public async Task ExecuteAsync(string userName, CancellationToken cancellationToken)
    {
        var userToDelete = await usersRepository.GetUserByNameAsync(userName, cancellationToken);
        if (userToDelete is null)
        {
            throw new UserNotFoundByNameException(userName);
        }

        await usersRepository.DeleteUserAsync(userToDelete);
    }
}