using ProblemDetailsExample.Controllers.V1.Model.Requests;
using ProblemDetailsExample.Controllers.V1.Model.Requests.Enums;

namespace ProblemDetailsExample.Handlers.Interfaces;

public interface ITransactionHandlerResolver
{
    ITransactionHandler GetTransactionHandler(TransactionType type);
}