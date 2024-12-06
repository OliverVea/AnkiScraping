namespace Olve.Operations;

public interface IAsyncOperation<in TInput>
{
    Task ExecuteAsync(TInput input, CancellationToken ct = default);
}

public interface IOperation<in TRequest, out TResult>
{
    TResult Execute(TRequest request);
}

public interface IAsyncOperation<in TRequest, TResult>
{
    Task<TResult> ExecuteAsync(TRequest request, CancellationToken ct = default);
}