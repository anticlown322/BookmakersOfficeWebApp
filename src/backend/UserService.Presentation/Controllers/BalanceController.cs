using System.Text.Json;
using Domain.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation;
using UserService.Domain.RequestFeatures;
using UserService.Presentation.Utility;

namespace UserService.Presentation.Controllers;

[Route("api/users/{username}/balance")]
public class BalanceController(
    IGetUserBalanceUseCase getUserBalanceUseCase,
    IDepositToUserBalanceUseCase depositToUserBalanceUseCase,
    IWithdrawFromUserBalanceUseCase withdrawFromUserBalanceUseCase,
    IGetTransactionHistory getTransactionHistory)
    : ControllerBase
{
    [HttpGet("current")]
    [Authorize(Policy= AuthorizationPolicies.AdministratorOrModeratorOrGambler)]
    public async Task<IActionResult> GetCurrentBalance(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        var userBalance = await getUserBalanceUseCase.ExecuteAsync(username, cancellationToken);

        return Ok(userBalance);
    }

    [HttpPost("deposit")]
    [ValidationFilter<DepositRequestDto>]
    public async Task Deposit(
        [FromRoute] string username,
        [FromBody] DepositRequestDto dto,
        CancellationToken cancellationToken)
    {
        await depositToUserBalanceUseCase.ExecuteAsync(username, dto, cancellationToken);

        NoContent();
    }

    [HttpPost("withdraw")]
    [Authorize(Policy= AuthorizationPolicies.GamblerOnly)]
    [ValidationFilter<WithdrawRequestDto>]
    public async Task Withdraw(
        [FromRoute] string username,
        [FromBody] WithdrawRequestDto dto,
        CancellationToken cancellationToken)
    {
        await withdrawFromUserBalanceUseCase.ExecuteAsync(username, dto, cancellationToken);

        NoContent();
    }

    [HttpGet("history")]
    [Authorize(Policy= AuthorizationPolicies.AdministratorOrModeratorOrGambler)]
    public async Task<IActionResult> GetTransactionHistory(
        [FromRoute] string username,
        [FromQuery] TransactionParameters transactionParameters,
        CancellationToken cancellationToken)
    {
        var pagedResult = await getTransactionHistory.ExecuteAsync(username, transactionParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.transactions);
    }
}