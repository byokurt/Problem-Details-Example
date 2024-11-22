namespace ProblemDetailsExample.Services.Interfaces;

public interface IOutboxMessagePublisherService
{
    Task Publish(CancellationToken cancellationToken);
}