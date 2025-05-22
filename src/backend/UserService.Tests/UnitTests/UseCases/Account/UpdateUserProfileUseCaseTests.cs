using AutoMapper;
using UserService.Application.DTO.Account;
using UserService.Application.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Account;

public class UpdateUserProfileUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateUserProfileUseCase _updateUserProfileUseCase;

    public UpdateUserProfileUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _mapperMock = new Mock<IMapper>();
        _updateUserProfileUseCase = new UpdateUserProfileUseCase(_usersRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidUpdate_UpdatesUserProfile()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithProfile();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var updateDto = new UserProfileUpdateDto 
        { 
            FirstName = "NewFirstName", 
            LastName = "NewLastName" 
        };

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(x => x.Map(updateDto, user))
            .Verifiable();

        // Act
        await _updateUserProfileUseCase.ExecuteAsync(username, updateDto, ct);

        // Assert
        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(username, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.UpdateUserAsync(user, ct), Times.Once);
        _mapperMock.Verify(x => x.Map(updateDto, user), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var ct = CancellationToken.None;
        var updateDto = new UserProfileUpdateDto();

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _updateUserProfileUseCase.ExecuteAsync(username, updateDto, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");

    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = "testUser";
        var ct = new CancellationToken(canceled: true);
        var updateDto = new UserProfileUpdateDto();

        // Act
        Func<Task> act = () => _updateUserProfileUseCase.ExecuteAsync(username, updateDto, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithProfile();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var updateDto = new UserProfileUpdateDto();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map(updateDto, user))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _updateUserProfileUseCase.ExecuteAsync(username, updateDto, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();
            
        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryUpdateThrows_PropagatesException()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithProfile();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var updateDto = new UserProfileUpdateDto();
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _updateUserProfileUseCase.ExecuteAsync(username, updateDto, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();
            
        exception.Which.Message.Should().Be(expectedException.Message);
    }
}