using CloudCart.Contracts.Requests;
using CloudCart.Contracts.Responses;
using CloudCart.Domain.Entities;
using CloudCart.Domain.ValueObjects;

namespace CloudCart.Application.Mappers;

public static class OrderMapper
{
    public static OrderResponse ToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            ShippingAddress = ToAddressResponse(order.ShippingAddress),
            OrderItems = order.OrderItems?.Select(ToOrderItemResponse).ToList() ?? new List<OrderItemResponse>()
        };
    }

    public static List<OrderResponse> ToResponseList(IEnumerable<Order> orders)
    {
        return orders.Select(ToResponse).ToList();
    }

    public static OrderItemResponse ToOrderItemResponse(OrderItem orderItem)
    {
        return new OrderItemResponse
        {
            Id = orderItem.Id,
            ProductId = orderItem.ProductId,
            ProductName = orderItem.Product?.Name ?? string.Empty,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
            TotalPrice = orderItem.TotalPrice
        };
    }

    public static AddressResponse ToAddressResponse(Address address)
    {
        return new AddressResponse
        {
            Street = address.Street,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country
        };
    }

    public static Address ToAddressEntity(AddressRequest request)
    {
        return Address.Create(
            request.Street,
            request.City,
            request.State,
            request.ZipCode,
            request.Country
        );
    }
}
