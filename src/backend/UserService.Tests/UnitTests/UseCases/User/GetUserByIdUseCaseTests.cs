using AutoMapper;
using UserService.Application.DTO.User;
using UserService.Application.UseCases.User;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.User;

public class GetUserByIdUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserByIdUseCase _getUserByIdUseCase;

    public GetUserByIdUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _mapperMock = new Mock<IMapper>();
        _getUserByIdUseCase = new GetUserByIdUseCase(_usersRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_UserExists_ReturnsMappedUserDto()
    {
        // Arrange
        var userId = UserUseCasesTestData.ValidUserId;
        var user = UserUseCasesTestData.ValidUser;
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByIdAsync(userId, ct))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<UserGetDto>(user))
            .Returns(UserUseCasesTestData.ValidUserDto);

        // Act
        var result = await _getUserByIdUseCase.ExecuteAsync(userId, ct);

        // Assert
        result.UserName.Should().Be(user.UserName);

        _usersRepositoryMock.Verify(
            x => x.GetUserByIdAsync(userId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<UserGetDto>(user),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotExists_ThrowsUserNotFoundByIdException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByIdAsync(userId, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _getUserByIdUseCase.ExecuteAsync(userId, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByIdException>()
            .WithMessage($"The user with id: {userId} does not exist in the database.");

        _usersRepositoryMock.Verify(
            x => x.GetUserByIdAsync(userId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<UserGetDto>(It.IsAny<Domain.Models.User>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getUserByIdUseCase.ExecuteAsync(userId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();


        _usersRepositoryMock.Verify(
            x => x.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByIdAsync(userId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getUserByIdUseCase.ExecuteAsync(userId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var userId = UserUseCasesTestData.ValidUserId;
        var ct = CancellationToken.None;
        var user = UserUseCasesTestData.ValidUser;
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _usersRepositoryMock
            .Setup(x => x.GetUserByIdAsync(userId, ct))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(x => x.Map<UserGetDto>(user))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getUserByIdUseCase.ExecuteAsync(userId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}