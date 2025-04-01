using UserService.Application.DTO.Account;

namespace UserService.Application.Contracts.UseCases.Account;

public interface IResetPasswordUseCase
{
    public Task ExecuteAsync(string username, PasswordResetDto passwordResetDto, CancellationToken cancellationToken);
}