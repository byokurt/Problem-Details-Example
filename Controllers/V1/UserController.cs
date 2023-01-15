using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProblemDetailsExample.Controllers.V1.Model.Request;
using ProblemDetailsExample.Controllers.V1.Model.Response;
using ProblemDetailsExample.Data;
using ProblemDetailsExample.Data.Entities;
using ProblemDetailsExample.Extensions;
using ProblemDetailsExample.Filters;
using ProblemDetailsExample.Models.Pagination;
using ProblemDetailsExample.V1.Controllers.Model.Request;

namespace ProblemDetailsExample.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("v{version:apiVersion}/users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    private readonly DemoDbContext _demoDbContext;

    public UserController(ILogger<UserController> logger, DemoDbContext demoDbContext)
    {
        _logger = logger;
        _demoDbContext = demoDbContext;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Get(int id)
    {
        User? user = await _demoDbContext.Users.FirstOrDefaultAsync(w => w.Id == id);

        if (user == null)
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

        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Post(CreateUserRequest request)
    {
        User user = new User();

        user.Name = request.Name;
        user.Surename = request.Surename;

        _demoDbContext.Users.Add(user);

        await _demoDbContext.SaveChangesAsync();

        return Created("", user.Id);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Query(QueryUsersRequest request)
    {
        IQueryable<User> query = _demoDbContext.Users.AsNoTracking().Where(w => w.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(w => w.Name == request.Name);
        }

        if (!string.IsNullOrWhiteSpace(request.Surename))
        {
            query = query.Where(w => w.Surename == request.Surename);
        }

        if (request.Id != null)
        {
            query = query.Where(w => w.Id == request.Id);
        }

        request.Order = PaginationOrderType.Asc;
        request.OrderBy = nameof(Data.Entities.User.Name);

        IPage<QueryUsersResponse> result = await query.Select(x => new QueryUsersResponse
        {
            Id = x.Id,
            Title = x.Name,
            IsDeleted = x.IsDeleted
        }).ToPageAsync(request);

        return new PageResult<QueryUsersResponse>(result);
    }
}