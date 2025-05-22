using AutoMapper;
using UserService.Application.DTO.Account;
using UserService.Application.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;
using UserService.Infrastructure.Utility;

namespace UserService.Tests.UnitTests.UseCases.Account;

public class GetUserProfileUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserProfileUseCase _getUserProfileUseCase;

    public GetUserProfileUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _mapperMock = new Mock<IMapper>();
        _getUserProfileUseCase = new GetUserProfileUseCase(_usersRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidUser_ReturnsProfileWithRoles()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithProfile();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var roles = new List<string> { UserRoles.Administrator, UserRoles.Gambler };
        var expectedDto = UseCasesTestData.ValidUserProfileDto;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.GetUserRolesAsync(user, ct))
            .ReturnsAsync(roles);
        
        _mapperMock
            .Setup(x => x.Map<UserProfileGetDto>(
                user, 
                It.IsAny<Action<IMappingOperationOptions<object, UserProfileGetDto>>>()))
            .Returns((Domain.Models.User src, Action<IMappingOperationOptions<object, UserProfileGetDto>> opts) =>
            {
                var optionsMock = new Mock<IMappingOperationOptions<object, UserProfileGetDto>>();
                var itemsDict = new Dictionary<string, object>();
            
                optionsMock
                    .Setup(o => o.Items)
                    .Returns(itemsDict);
                
                opts(optionsMock.Object);
            
                itemsDict.Should().ContainKey("Roles")
                    .And.Subject["Roles"].Should().BeEquivalentTo(roles);
            
                return new UserProfileGetDto
                {
                    FirstName = src.Profile.FirstName,
                    LastName = src.Profile.LastName,
                    Roles = (List<string>)itemsDict["Roles"]
                };
            });
        
        // Act
        var result = await _getUserProfileUseCase.ExecuteAsync(username, ct);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(expectedDto.FirstName);
        result.LastName.Should().Be(expectedDto.LastName);
        result.Roles.Should().BeEquivalentTo(roles);
        
        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(username, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.GetUserRolesAsync(user, ct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _getUserProfileUseCase.ExecuteAsync(username, ct);

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

        // Act
        Func<Task> act = () => _getUserProfileUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var username = "testUser";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getUserProfileUseCase.ExecuteAsync(username, ct);

        // Assert
        var exception = await act.Should()
            .ThrowAsync<Exception>();
            
        exception.Which.Message.Should().Be(expectedException.Message);
    }
}