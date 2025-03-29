using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;

namespace UserService.Presentation.Controllers;

[Route("api/users/{username}/balance")]
public class BalanceController(
    IGetUserBalanceUseCase getUserBalanceUseCase,
    IDepositToUserBalanceUseCase depositToUserBalanceUseCase,
    IWithDrawFromUserBalanceUseCase withdrawFromUserBalanceUseCase,
    IGetTransactionHistory getTransactionHistory)
    : ControllerBase
{
    [HttpGet("current")]
    [Authorize(Policy= "AdministratorOrModeratorOrGambler")]
    public async Task<IActionResult> GetCurrentBalance(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        var userBalance = await getUserBalanceUseCase.ExecuteAsync(username, cancellationToken);

        return Ok(userBalance);
    }

    [HttpPost("deposit")]
    public async Task Deposit(
        [FromRoute] string username,
        [FromBody] DepositRequestDto dto,
        CancellationToken cancellationToken)
    {
        await depositToUserBalanceUseCase.ExecuteAsync(username, dto, cancellationToken);

        NoContent();
    }

    [HttpPost("withdraw")]
    [Authorize(Policy= "GamblerOnly")]
    public async Task Withdraw(
        [FromRoute] string username,
        [FromBody] WithdrawRequestDto dto,
        CancellationToken cancellationToken)
    {
        await withdrawFromUserBalanceUseCase.ExecuteAsync(username, dto, cancellationToken);

        NoContent();
    }

    [HttpGet("history")]
    [Authorize(Policy= "AdministratorOrModeratorOrGambler")]
    public async Task<IActionResult> GetTransactionHistory(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        var history = await getTransactionHistory.ExecuteAsync(username, cancellationToken);

        return Ok(history);
    }
}