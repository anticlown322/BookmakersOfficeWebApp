namespace BettingService.BLL.Exceptions.Specific;

public class KafkaProduceException(string message, Exception innerException) : Exception(message, innerException);