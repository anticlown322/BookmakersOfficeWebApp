namespace BettingService.BLL.Exceptions.Base;

public abstract class UnauthorizedException(string message) : Exception(message);