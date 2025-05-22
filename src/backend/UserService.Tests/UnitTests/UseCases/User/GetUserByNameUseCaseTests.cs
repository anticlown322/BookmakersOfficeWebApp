using AutoMapper;
using UserService.Application.DTO.User;
using UserService.Application.UseCases.User;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.User;

public class GetUserByNameUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserByNameUseCase _getUserByNameUseCase;

    public GetUserByNameUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _mapperMock = new Mock<IMapper>();
        _getUserByNameUseCase = new GetUserByNameUseCase(_usersRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_UserExists_ReturnsMappedUserDto()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        var userDto = UseCasesTestData.ValidUserDto;
        var username = user.UserName;
        var ct = CancellationToken.None;
        
        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<UserGetDto>(user))
            .Returns(userDto);

        // Act
        var result = await _getUserByNameUseCase.ExecuteAsync(username, ct);

        // Assert
        result.UserName.Should().Be(userDto.UserName);
        
        _usersRepositoryMock.Verify(
            x => x.GetUserByNameAsync(username, ct),
            Times.Once);
        
        _mapperMock.Verify(
            x => x.Map<UserGetDto>(user),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotExists_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var ct = CancellationToken.None;
        
        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _getUserByNameUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");

        
        _usersRepositoryMock.Verify(
            x => x.GetUserByNameAsync(username, ct),
            Times.Once);
        
        _mapperMock.Verify(
            x => x.Map<UserGetDto>(It.IsAny<Domain.Models.User>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = UseCasesTestData.ValidUser.UserName;
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getUserByNameUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
        
        _usersRepositoryMock.Verify(
            x => x.GetUserByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var username = UseCasesTestData.ValidUser.UserName;
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getUserByNameUseCase.ExecuteAsync(username, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        var username = user.UserName;
        var ct = CancellationToken.None;
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<UserGetDto>(user))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getUserByNameUseCase.ExecuteAsync(username, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);

    }
}