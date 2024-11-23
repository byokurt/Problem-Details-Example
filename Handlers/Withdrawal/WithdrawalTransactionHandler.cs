using ProblemDetailsExample.Controllers.V1.Model.Requests;
using ProblemDetailsExample.Controllers.V1.Model.Requests.Enums;
using ProblemDetailsExample.Handlers.Interfaces;

namespace ProblemDetailsExample.Handlers.Withdrawal;

public class WithdrawalTransactionHandler : ITransactionHandler
{
    public WithdrawalTransactionHandler()
    {
        
    }
    
    public TransactionType Type => TransactionType.Withdrawal;
    
    public async Task<Guid> Execute(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        Guid transactionId = Guid.NewGuid();
        
        return transactionId;
    }
}