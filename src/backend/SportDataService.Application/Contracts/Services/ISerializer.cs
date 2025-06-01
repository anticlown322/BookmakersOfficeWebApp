namespace SportDataService.Application.Contracts.Services;

public interface ISerializer
{
    byte[] Serialize<T>(T value);
    T Deserialize<T>(byte[] value);
}