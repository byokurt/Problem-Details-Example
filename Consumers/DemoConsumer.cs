using MassTransit;
using ProblemDetailsExample.Events;

namespace ProblemDetailsExample.Consumers;

public class DemoConsumer : IConsumer<DemoEvent>
{
    private readonly ILogger<DemoConsumer> _logger;

    public DemoConsumer(ILogger<DemoConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DemoEvent> context)
    {
        _logger.LogInformation("Running Consumer");

        await Task.CompletedTask;
    }
}