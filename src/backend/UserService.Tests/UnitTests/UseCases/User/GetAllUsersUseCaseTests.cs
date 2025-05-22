using AutoMapper;
using UserService.Application.DTO.User;
using UserService.Application.UseCases.User;
using UserService.Domain.RepositoryContracts;
using UserService.Domain.RequestFeatures;

namespace UserService.Tests.UnitTests.UseCases.User;

public class GetAllUsersUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllUsersUseCase _getAllUsersUseCase;

    public GetAllUsersUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _mapperMock = new Mock<IMapper>();
        _getAllUsersUseCase = new GetAllUsersUseCase(_usersRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMappedUsersAndMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var userParams = new UserParameters();
        var testUsers = UseCasesTestData.CreateTestUsers(3);
        var testUserDtos = UseCasesTestData.CreateTestUserDtos(3);

        _usersRepositoryMock
            .Setup(x => x.GetAllUsersAsync(userParams, ct))
            .ReturnsAsync(testUsers);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<UserGetDto>>(testUsers))
            .Returns(testUserDtos);

        // Act
        var result = await _getAllUsersUseCase.ExecuteAsync(userParams, ct);

        // Assert
        result.users.Should().BeEquivalentTo(testUserDtos);
        result.metaData.Should().Be(testUsers.MetaData);

        _usersRepositoryMock.Verify(
            x => x.GetAllUsersAsync(userParams, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<UserGetDto>>(testUsers),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyResult_ReturnsEmptyCollectionWithMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var userParams = new UserParameters();
        var emptyUsers = UseCasesTestData.CreateTestUsers(0);
        var emptyDtos = UseCasesTestData.CreateTestUserDtos(0);

        _usersRepositoryMock
            .Setup(x => x.GetAllUsersAsync(userParams, ct))
            .ReturnsAsync(emptyUsers);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<UserGetDto>>(emptyUsers))
            .Returns(emptyDtos);

        // Act
        var result = await _getAllUsersUseCase.ExecuteAsync(userParams, ct);

        // Assert
        result.users.Should().BeEmpty();
        result.metaData.Should().Be(emptyUsers.MetaData);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var ct = new CancellationToken(canceled: true);
        var userParams = new UserParameters();

        // Act
        Func<Task> act = () => _getAllUsersUseCase.ExecuteAsync(userParams, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _usersRepositoryMock.Verify(
            x => x.GetAllUsersAsync(It.IsAny<UserParameters>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var userParams = new UserParameters();
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetAllUsersAsync(userParams, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getAllUsersUseCase.ExecuteAsync(userParams, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var userParams = new UserParameters();
        var testUsers = UseCasesTestData.CreateTestUsers(2);
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _usersRepositoryMock
            .Setup(x => x.GetAllUsersAsync(userParams, ct))
            .ReturnsAsync(testUsers);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<UserGetDto>>(testUsers))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getAllUsersUseCase.ExecuteAsync(userParams, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}