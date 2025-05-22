using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Options;
using UserService.Domain.Models;
using UserService.Infrastructure.Services;

namespace UserService.Tests.UnitTests.Services;

public class EmailServiceTests
{
    private readonly Mock<IFluentEmail> _fluentEmailMock = new();
    private readonly EmailSettings _emailSettings = new()
    {
        SenderEmail = "noreply@test.com",
        SenderName = "Test Service"
    };

    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        var optionsMock = new Mock<IOptions<EmailSettings>>();
        optionsMock.Setup(x => x.Value).Returns(_emailSettings);

        _emailService = new EmailService(optionsMock.Object, _fluentEmailMock.Object);
    }

    [Fact]
    public async Task SendConfirmationEmailAsync_ShouldBuildEmailCorrectly()
    {
        // Arrange
        const string email = "user@test.com";
        const string confirmationLink = "https://example.com/confirm?token=123";
        const string expectedHtmlBody = $"Please, follow this link to confirm your account: <a href='{confirmationLink}'>Confirm</a>";

        // Настраиваем цепочку вызовов:
        _fluentEmailMock
            .Setup(x => x.To(It.IsAny<string>()))
            .Returns(_fluentEmailMock.Object);

        _fluentEmailMock
            .Setup(x => x.Subject(It.IsAny<string>()))
            .Returns(_fluentEmailMock.Object);

        _fluentEmailMock
            .Setup(x => x.Body(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(_fluentEmailMock.Object);

        _fluentEmailMock
            .Setup(x => x.SendAsync(null))
            .ReturnsAsync(new SendResponse());

        // Act
        await _emailService.SendConfirmationEmailAsync(email, confirmationLink);

        // Assert
        _fluentEmailMock.Verify(x => x.To(email), Times.Once);
        _fluentEmailMock.Verify(x => x.Subject("Account confirmation"), Times.Once);
        _fluentEmailMock.Verify(x => x.Body(expectedHtmlBody, true), Times.Once);
        _fluentEmailMock.Verify(x => x.SendAsync(It.IsAny<CancellationToken?>()), Times.Once);
    }

    [Fact]
    public async Task SendResetPasswordEmailAsync_ShouldBuildEmailCorrectly()
    {
        // Arrange
        const string email = "user@test.com";
        const string resetCode = "ABC123";

        // Настраиваем цепочку вызовов:
        _fluentEmailMock
            .Setup(x => x.To(It.IsAny<string>()))
            .Returns(_fluentEmailMock.Object);

        _fluentEmailMock
            .Setup(x => x.Subject(It.IsAny<string>()))
            .Returns(_fluentEmailMock.Object);

        _fluentEmailMock
            .Setup(x => x.Body(It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(_fluentEmailMock.Object);

        _fluentEmailMock
            .Setup(x => x.SendAsync(null))
            .ReturnsAsync(new SendResponse());

        // Act
        await _emailService.SendResetPasswordEmailAsync(email, resetCode);

        // Assert
        _fluentEmailMock.Verify(x => x.To(email), Times.Once);
        _fluentEmailMock.Verify(x => x.Subject("Password reset code"), Times.Once);
        _fluentEmailMock.Verify(x => x.Body($"Password reset code: {resetCode}", false), Times.Once);
        _fluentEmailMock.Verify(x => x.SendAsync(It.IsAny<CancellationToken?>()), Times.Once);
    }
}