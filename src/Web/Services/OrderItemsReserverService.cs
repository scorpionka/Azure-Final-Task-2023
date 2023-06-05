using System.Text;
using System.Text.Json;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Web.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Microsoft.eShopWeb.Web.Services;

public class OrderItemsReserverService : IOrderItemsReserverService
{
    public OrderItemsReserverService()
    {
    }

    public async Task PostOrderItemsReserverAsync(Order order)
    {
        string connectionString = "Endpoint=sb://eshoporder.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey={key}";
        string queueName = "ordermessages";

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
    }
}
