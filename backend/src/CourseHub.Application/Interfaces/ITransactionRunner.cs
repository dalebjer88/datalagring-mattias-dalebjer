namespace CourseHub.Application.Interfaces;

public interface ITransactionRunner
{
    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default);
}
