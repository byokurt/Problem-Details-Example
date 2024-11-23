using Microsoft.AspNetCore.Mvc;
using ProblemDetailsExample.Controllers.V1.Model.Requests;
using ProblemDetailsExample.Handlers.Interfaces;

namespace ProblemDetailsExample.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("v{version:apiVersion}/transactions")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly ITransactionHandlerResolver _transactionHandlerResolver;

    public TransactionController(
        ILogger<TransactionController> logger,
        ITransactionHandlerResolver transactionHandlerResolver)
    {
        _logger = logger;
        _transactionHandlerResolver = transactionHandlerResolver;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Post(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        ITransactionHandler transactionHandler = _transactionHandlerResolver.GetTransactionHandler(request.Type);
        
        Guid transactionId = await transactionHandler.Execute(request, cancellationToken);
        
        return Created("/transactions/", transactionId);
    }
}