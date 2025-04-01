namespace UserService.Application.DTO.Authentication;

public record TokensGetDto(string AccessToken, string RefreshToken);