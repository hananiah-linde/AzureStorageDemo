namespace AzureStorageDemo.Interfaces;

public interface IAzureStorageService
{
    Task AddQueueMessageAsync<T>(string queueName, T data);
    Task AddQueueMessagesAsync<T>(string queueName, IEnumerable<T> data);
}