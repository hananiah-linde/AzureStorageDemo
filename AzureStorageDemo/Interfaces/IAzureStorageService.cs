namespace AzureStorageDemo.Interfaces;

public interface IAzureStorageService
{
    Task AddQueueMessageAsync<T>(string queueName, T data);
}