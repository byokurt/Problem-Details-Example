using System.Text.Json;
using ProblemDetailsExample.Data.Entities;
using ProblemDetailsExample.Data.Enums;
using ProblemDetailsExample.Factories.Interfaces;

namespace ProblemDetailsExample.Factories;

public class OutboxMessageFactory : IOutboxMessageFactory
{
    public OutboxMessage From<T>(T @event, DateTime now, string queueName = null) where T : class, new()
    {
        string data = JsonSerializer.Serialize(@event);

        string type = @event.GetType().FullName;

        OutboxMessage outboxMessage = new OutboxMessage
        {
            Data = data,
            OccurredOn = now,
            Status = OutboxMessageStatus.New,
            Type = type,
            QueueName = queueName
        };

        return outboxMessage;
    }
}