using CloudCart.Contracts.Responses;
using FluentValidation;
using System.Text.Json;

namespace CloudCart.API.Middleware;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;

    public ValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        await _next(context);
    }
}
