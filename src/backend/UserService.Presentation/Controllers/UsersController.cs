using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.DTO;
using UserService.Application.DTO.User;
using UserService.Domain.RequestFeatures;

namespace UserService.Presentation.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController(
    IGetAllUsersUseCase getAllUsersUseCase,
    IGetUserByIdUseCase getUserByIdUseCase,
    IGetUserByNameUseCase getUserByNameUseCase,
    IDeleteUserUseCase deleteUserUseCase)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Policy= "AdministratorOrModerator")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] UserParameters userParameters,
        CancellationToken cancellationToken)
    {
        var pagedResult = await getAllUsersUseCase.ExecuteAsync(userParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.users);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy= "AdministratorOrModerator")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var eventToGet = await getUserByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(eventToGet);
    }

    [HttpGet("{username}")]
    [Authorize(Policy= "AdministratorOrModerator")]
    public async Task<IActionResult> GetUserByName(string username, CancellationToken cancellationToken)
    {
        var eventToGet = await getUserByNameUseCase.ExecuteAsync(username, cancellationToken);

        return Ok(eventToGet);
    }

    [HttpDelete("{username}")]
    [Authorize(Policy= "AdministratorOrGambler")]
    public async Task<IActionResult> DeleteUser(string username, CancellationToken cancellationToken)
    {
        await deleteUserUseCase.ExecuteAsync(username, cancellationToken);

        return NoContent();
    }
}