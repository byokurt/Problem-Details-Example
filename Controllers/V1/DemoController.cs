using Microsoft.AspNetCore.Mvc;
using ProblemDetailsExample.Filters;
using ProblemDetailsExample.V1.Controllers.Model.Request;

namespace ProblemDetailsExample.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("v{version:apiVersion}/demos")]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;

    public DemoController(ILogger<DemoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public string Get()
    {
        ProblemDetails problemDetails = new ProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Title = "User Not Found.",
            Type = "user-not-found",
            Detail = "There is no record at Db for the id",
            Extensions =
            {
                new KeyValuePair<string, object?>("Id", 1)
            }
        };

        throw new ProblemDetailsException(problemDetails);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public string Post(Demo request)
    {
        return "Ok";
    }
}