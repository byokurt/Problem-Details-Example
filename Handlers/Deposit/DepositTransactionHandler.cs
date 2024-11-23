using ProblemDetailsExample.Controllers.V1.Model.Requests;
using ProblemDetailsExample.Controllers.V1.Model.Requests.Enums;
using ProblemDetailsExample.Handlers.Interfaces;

namespace ProblemDetailsExample.Handlers.Deposit;

public class DepositTransactionHandler : ITransactionHandler
{
    public DepositTransactionHandler()
    {
        
    }

    public TransactionType Type => TransactionType.Deposit;
    
    public async Task<Guid> Execute(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        Guid transactionId = Guid.NewGuid();
        
        return transactionId;
    }
}