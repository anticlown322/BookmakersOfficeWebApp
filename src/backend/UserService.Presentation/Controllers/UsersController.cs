using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.DTO;
using UserService.Application.DTO.User;
using UserService.Domain.RequestFeatures;
using UserService.Presentation.Utility;

namespace UserService.Presentation.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController(
    IGetAllUsersUseCase getAllUsersUseCase,
    IGetUserByIdUseCase getUserByIdUseCase,
    IGetUserByNameUseCase getUserByNameUseCase,
    IDeleteUserUseCase deleteUserUseCase,
    ILogger<UsersController> logger)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOrModerator)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] UserParameters userParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all users...");

        var pagedResult = await getAllUsersUseCase.ExecuteAsync(userParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.users);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOrModerator)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user by id...");

        var eventToGet = await getUserByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(eventToGet);
    }

    [HttpGet("{username}")]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOrModerator)]
    public async Task<IActionResult> GetUserByName(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user by name...");

        var eventToGet = await getUserByNameUseCase.ExecuteAsync(username, cancellationToken);

        return Ok(eventToGet);
    }

    [HttpDelete("{username}")]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOrModerator)]
    public async Task<IActionResult> DeleteUser(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting user by id...");

        await deleteUserUseCase.ExecuteAsync(username, cancellationToken);

        return NoContent();
    }
}