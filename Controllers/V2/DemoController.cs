using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProblemDetailsExample.Controllers.V2.Model.Request;
using ProblemDetailsExample.Controllers.V2.Model.Response;
using ProblemDetailsExample.Data;
using ProblemDetailsExample.Data.Entities;
using ProblemDetailsExample.Extensions;
using ProblemDetailsExample.Filters;
using ProblemDetailsExample.Models.Pagination;
using ProblemDetailsExample.V2.Controllers.Model.Request;

namespace ProblemDetailsExample.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[Produces("application/json")]
[Route("v{version:apiVersion}/demos")]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;

    private readonly DemoDbContext _demoDbContext;

    public DemoController(ILogger<DemoController> logger, DemoDbContext demoDbContext)
    {
        _logger = logger;
        _demoDbContext = demoDbContext;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Query(GetUsersRequest request)
    {
        IQueryable<User> query = _demoDbContext.Users.AsNoTracking().Where(w => w.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            query = query.Where(w => w.Title == request.Title);
        }

        if (request.Id != null)
        {
            query = query.Where(w => w.Id == request.Id);
        }

        request.Order = PaginationOrderType.Asc;
        request.OrderBy = nameof(Data.Entities.User.Title);

        IPage<GetUsersResponse> result = await query.Select(x => new GetUsersResponse
        {
            Id = x.Id,
            Title = x.Title,
            IsDeleted = x.IsDeleted
        }).ToPageAsync(request);

        return new PageResult<GetUsersResponse>(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public string Post(Demo request)
    {
        return "Ok";
    }
}