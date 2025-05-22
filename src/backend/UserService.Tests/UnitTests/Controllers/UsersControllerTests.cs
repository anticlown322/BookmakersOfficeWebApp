using System.Text.Json;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.DTO.User;
using UserService.Domain.RequestFeatures;
using UserService.Presentation.Controllers;

namespace UserService.Tests.UnitTests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IGetAllUsersUseCase> _getAllUsersUseCaseMock = new();
    private readonly Mock<IGetUserByIdUseCase> _getUserByIdUseCaseMock = new();
    private readonly Mock<IGetUserByNameUseCase> _getUserByNameUseCaseMock = new();
    private readonly Mock<IDeleteUserUseCase> _deleteUserUseCaseMock = new();
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _controller = new UsersController(
            _getAllUsersUseCaseMock.Object,
            _getUserByIdUseCaseMock.Object,
            _getUserByNameUseCaseMock.Object,
            _deleteUserUseCaseMock.Object);

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetUsers_ShouldReturnOkWithPagedUsers_WhenSuccess()
    {
        // Arrange
        var userParameters = new UserParameters();
        var users = new List<UserGetDto>
        {
            new() { UserName = "user1" },
            new() { UserName = "user2" }
        };
        var metaData = new MetaData
        {
            CurrentPage = 1,
            PageSize = 10,
            TotalCount = 2,
            TotalPages = 1
        };

        _getAllUsersUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<UserParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, metaData));

        // Act
        var result = await _controller.GetUsers(userParameters, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(users);

        _controller.HttpContext.Response.Headers.Should().ContainKey("X-Pagination");
        var paginationHeader = _controller.HttpContext.Response.Headers["X-Pagination"].First();
        var deserializedMetaData = JsonSerializer.Deserialize<MetaData>(paginationHeader);
        deserializedMetaData.Should().BeEquivalentTo(metaData);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnOkWithUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new UserGetDto { UserName = "testuser" };

        _getUserByIdUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.GetUserById(userId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetUserByName_ShouldReturnOkWithUser_WhenUserExists()
    {
        // Arrange
        const string username = "testuser";
        var expectedUser = new UserGetDto { UserName = username };

        _getUserByNameUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.GetUserByName(username, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNoContent_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        _deleteUserUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteUser(username, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetUsers_ShouldCallUseCaseWithCorrectParameters()
    {
        // Arrange
        var userParameters = new UserParameters { PageNumber = 2, PageSize = 20 };
        var cancellationToken = CancellationToken.None;

        _getAllUsersUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<UserParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserGetDto>(), new MetaData()));

        // Act
        await _controller.GetUsers(userParameters, cancellationToken);

        // Assert
        _getAllUsersUseCaseMock.Verify(x =>
            x.ExecuteAsync(userParameters, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ShouldCallUseCaseWithCorrectId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _getUserByIdUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserGetDto());

        // Act
        await _controller.GetUserById(userId, cancellationToken);

        // Assert
        _getUserByIdUseCaseMock.Verify(x =>
            x.ExecuteAsync(userId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetUsers_ShouldUseDefaultParameters_WhenNotProvided()
    {
        // Arrange
        var defaultParameters = new UserParameters();

        _getAllUsersUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<UserParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<UserGetDto>(), new MetaData()));

        // Act
        await _controller.GetUsers(defaultParameters, CancellationToken.None);

        // Assert
        _getAllUsersUseCaseMock.Verify(x =>
                x.ExecuteAsync(
                    It.Is<UserParameters>(p =>
                        p.PageNumber == defaultParameters.PageNumber &&
                        p.PageSize == defaultParameters.PageSize),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }
}