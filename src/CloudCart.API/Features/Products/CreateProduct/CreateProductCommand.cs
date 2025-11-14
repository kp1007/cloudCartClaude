using CloudCart.Application.Abstractions;
using CloudCart.Contracts.Requests;
using CloudCart.Contracts.Responses;

namespace CloudCart.API.Features.Products.CreateProduct;

public class CreateProductCommand : ICommand
{
    public CreateProductRequest Request { get; }

    public CreateProductCommand(CreateProductRequest request)
    {
        Request = request;
    }
}
