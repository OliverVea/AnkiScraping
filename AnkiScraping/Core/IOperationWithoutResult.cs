namespace AnkiScraping.Core;

public interface IOperationWithoutResult<in TInput>
{
    Task ExecuteAsync(TInput input, CancellationToken ct = default);
}