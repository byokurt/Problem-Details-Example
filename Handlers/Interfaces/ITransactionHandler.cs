using ProblemDetailsExample.Controllers.V1.Model.Requests;
using ProblemDetailsExample.Controllers.V1.Model.Requests.Enums;

namespace ProblemDetailsExample.Handlers.Interfaces;

public interface ITransactionHandler
{
    TransactionType Type { get; }
    
    Task<Guid> Execute(CreateTransactionRequest request, CancellationToken cancellationToken);
}