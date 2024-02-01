using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
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

    public async Task AddQueueMessagesAsync<T>(string queueName, IEnumerable<T> data)
    {
        var client = await GetQueueClient(queueName);

        var taskList = new List<Task>();
        foreach (var item in data)
        {
            var message = JsonSerializer.Serialize(item);
            taskList.Add(client.SendMessageAsync(message));
        }

        await Task.WhenAll(taskList);
    }

    public async Task UploadFileAsync(IFormFile file, string containerName)
    {
        var blobClient = GetBlobClient(containerName, file.FileName);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, true);
    }
    
    public async Task<byte[]> DownloadFileAsync(string containerName, string blobName)
    {
        var blobClient = GetBlobClient(containerName, blobName);

        var response = await blobClient.OpenReadAsync();
        using var streamReader = new StreamReader(response);
        var fileContent = Encoding.UTF8.GetBytes(await streamReader.ReadToEndAsync());
        return fileContent;
    }

    private BlobClient GetBlobClient(string containerName, string blobName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        return blobClient;
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