namespace CloudCart.Application.Abstractions;

/// <summary>
/// Handler for commands that modify state
/// </summary>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handler for commands that modify state and return data
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
