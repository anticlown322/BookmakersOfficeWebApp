namespace UserService.Application.DTO.Authentication;

public record TokenDto(string AccessToken, string RefreshToken);