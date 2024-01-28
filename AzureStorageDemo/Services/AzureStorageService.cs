using System.Text.Json;
using Azure.Storage.Queues;
using AzureStorageDemo.Interfaces;

namespace AzureStorageDemo.Services;

public class AzureStorageService(string connectionString) : IAzureStorageService
{
    public async Task AddQueueMessageAsync<T>(string queueName, T data)
    {
        var client = await GetQueueClient(queueName);
        var message = JsonSerializer.Serialize(data);

        await client.SendMessageAsync(message);
    }

    private async Task<QueueClient> GetQueueClient(string queueName)
    {
        var options = new QueueClientOptions
        {
            MessageEncoding = QueueMessageEncoding.Base64
        };

        var client = new QueueClient(connectionString, queueName, options);

        await client.CreateIfNotExistsAsync();
        
        return client;
    }
}