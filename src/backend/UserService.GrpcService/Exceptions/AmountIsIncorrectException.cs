using Grpc.Core;

namespace UserService.GrpcService.Exceptions;

public class AmountIsIncorrectException : GrpcExceptionBase
{
    public AmountIsIncorrectException(string details = "")
        : base(
            StatusCode.InvalidArgument,
            $"Invalid amount format{(string.IsNullOrEmpty(details) ? string.Empty : $": {details}")}")
    {
    }
}