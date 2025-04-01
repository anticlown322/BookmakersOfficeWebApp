namespace UserService.Application.DTO.Authentication;

public record TokensRefreshDto(string AccessToken, string RefreshToken);