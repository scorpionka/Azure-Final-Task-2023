using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Web.Pages.Basket;

namespace Microsoft.eShopWeb.Web.Interfaces;

public interface IOrderItemsReserverService
{
    public Task<string> PostOrderItemsReserverAsync(Order order);
}
