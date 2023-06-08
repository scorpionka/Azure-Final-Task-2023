using System.Text;
using System.Text.Json;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Web.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using System.Net.Http;

namespace Microsoft.eShopWeb.Web.Services;

public class OrderItemsReserverService : IOrderItemsReserverService
{
    private readonly HttpClient _httpClient;

    public OrderItemsReserverService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> PostOrderItemsReserverAsync(Order order)
    {
        string connectionString = "Endpoint=sb://order-items-reserver.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=";
        string queueName = "order-items-reserver-queue";

        await using var client = new ServiceBusClient(connectionString);
        ServiceBusSender sender = client.CreateSender(queueName);

        try
        {
            var message = new ServiceBusMessage(JsonSerializer.Serialize(order));

            await sender.SendMessageAsync(message);
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message, exception);
        }
        finally
        {
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }

        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            //RequestUri = new Uri("http://localhost:7058/api/DeliveryOrderProcessor")
            RequestUri = new Uri("https://deliveryorderprocessor2023.azurewebsites.net/api/DeliveryOrderProcessor")
        };

        request.Content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
