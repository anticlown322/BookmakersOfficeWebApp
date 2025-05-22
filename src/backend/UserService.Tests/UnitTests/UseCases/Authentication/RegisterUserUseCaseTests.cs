using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UserService.Application.DTO.Authentication;
using UserService.Application.UseCases.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Authentication;

public class RegisterUserUseCaseTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly RegisterUserUseCase _registerUserUseCase;

    public RegisterUserUseCaseTests()
    {
        _mapperMock = new Mock<IMapper>();
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _registerUserUseCase = new RegisterUserUseCase(_mapperMock.Object, _usersRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRegistration_ReturnsSuccess()
    {
        // Arrange
        var registrationDto = UseCasesTestData.ValidRegistrationDto;
        var user = UseCasesTestData.NewUser;
        var ct = CancellationToken.None;
        var successResult = IdentityResult.Success;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(registrationDto.UserName, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        _mapperMock
            .Setup(x => x.Map<Domain.Models.User>(registrationDto))
            .Returns(user);

        _usersRepositoryMock
            .Setup(x => x.CreateUserAsync(user, registrationDto.Password, registrationDto.Roles, ct))
            .ReturnsAsync(successResult);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _registerUserUseCase.ExecuteAsync(registrationDto, ct);
        
        result.Succeeded.Should().BeTrue();
        user.Profile.UserId.Should().Be(user.Id);
        user.Balance.UserId.Should().Be(user.Id);
        user.Balance.LastUpdated.Should().BeOnOrBefore(DateTime.UtcNow);

        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(registrationDto.UserName, ct), Times.Once);
        _mapperMock.Verify(x => x.Map<Domain.Models.User>(registrationDto), Times.Once);
        _usersRepositoryMock.Verify(x => x.CreateUserAsync(user, registrationDto.Password, registrationDto.Roles, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.UpdateUserAsync(user, ct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserAlreadyExists_ThrowsUserAlreadyExistsException()
    {
        // Arrange
        var registrationDto = UseCasesTestData.ValidRegistrationDto;
        var existingUser = UseCasesTestData.ExistingUser;
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(registrationDto.UserName, ct))
            .ReturnsAsync(existingUser);

        // Act
        Func<Task> act = () => _registerUserUseCase.ExecuteAsync(registrationDto, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserAlreadyExistsException>()
            .WithMessage($"User with {registrationDto.UserName} username already exists.");
    }

    [Fact]
    public async Task ExecuteAsync_CreateUserFails_ThrowsUserCanNonBeRegistered()
    {
        // Arrange
        var registrationDto = UseCasesTestData.ValidRegistrationDto;
        var user = UseCasesTestData.NewUser;
        var ct = CancellationToken.None;
        var error = new IdentityError { Description = "Password too weak" };
        var failedResult = IdentityResult.Failed(error);

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(registrationDto.UserName, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        _mapperMock
            .Setup(x => x.Map<Domain.Models.User>(registrationDto))
            .Returns(user);

        _usersRepositoryMock
            .Setup(x => x.CreateUserAsync(user, registrationDto.Password, registrationDto.Roles, ct))
            .ReturnsAsync(failedResult);

        // Act
        Func<Task> act = () => _registerUserUseCase.ExecuteAsync(registrationDto, ct);

        // Assert
        await act.Should().ThrowAsync<UserCanNonBeRegistered>();

    }
    
    [Fact]
    public async Task ExecuteAsync_EmptyRoles_CreatesUserWithEmptyRoles()
    {
        // Arrange
        var registrationDto = UseCasesTestData.ValidRegistrationDto with { Roles = null };
        var user = UseCasesTestData.NewUser;
        var ct = CancellationToken.None;
        var successResult = IdentityResult.Success;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(registrationDto.UserName, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        _mapperMock
            .Setup(x => x.Map<Domain.Models.User>(registrationDto))
            .Returns(user);

        _usersRepositoryMock
            .Setup(x => x.CreateUserAsync(user, registrationDto.Password, new List<string>(), ct))
            .ReturnsAsync(successResult);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _registerUserUseCase.ExecuteAsync(registrationDto, ct);

        // Assert
        result.Succeeded.Should().BeTrue();
        _usersRepositoryMock.Verify(x => x.CreateUserAsync(user, registrationDto.Password, new List<string>(), ct), Times.Once);
    }
}