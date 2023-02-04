using MassTransit;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProblemDetailsExample.Controllers.V1.Model.Request;
using ProblemDetailsExample.Controllers.V1.Model.Response;
using ProblemDetailsExample.Data;
using ProblemDetailsExample.Data.Entities;
using ProblemDetailsExample.Filters;
using ProblemDetailsExample.Models.Pagination;
using ProblemDetailsExample.V1.Controllers.Model.Request;
using ProblemDetailsExample.Events;
using Microsoft.Extensions.Caching.Distributed;
using ProblemDetailsExample.Extensions;

namespace ProblemDetailsExample.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("v{version:apiVersion}/users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly DemoDbContext _demoDbContext;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IBusControl _busControl;
    private readonly IDistributedCache _distributedCache;

    public UserController(ILogger<UserController> logger,
                          DemoDbContext demoDbContext,
                          ISendEndpointProvider sendEndpointProvider,
                          IBusControl busControl,
                          IDistributedCache distributedCache)
    {
        _logger = logger;
        _demoDbContext = demoDbContext;
        _sendEndpointProvider = sendEndpointProvider;
        _busControl = busControl;
        _distributedCache = distributedCache;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Query(QueryUsersRequest request)
    {
        IQueryable<User> query = _demoDbContext.Users.AsNoTracking();

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

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Get(int id)
    {
        User? user = await _distributedCache.Get<User>($"user_{id}");

        if (user == null)
        {
            user = await _demoDbContext.Users.FirstOrDefaultAsync(w => w.Id == id);

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

            await _distributedCache.Set($"user_{id}", user, TimeSpan.FromMinutes(5));
        }

        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Post(CreateUserRequest request)
    {
        User user = new User()
        {
            Name = request.Name,
            Surename = request.Surename
        };

        _demoDbContext.Users.Add(user);

        await _demoDbContext.SaveChangesAsync();

        await _busControl.Publish<DemoEvent>(new DemoEvent()
        {
            Name = request.Name,
            Surname = request.Surename
        });

        ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue_demo"));

        await sendEndpoint.Send(new DemoEvent() { Name = request.Name, Surname = request.Surename });

        return Created("/users/", user.Id);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(int id)
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

        _demoDbContext.Users.Remove(user);

        await _demoDbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/name")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] JsonPatchDocument<PatchUserRequest> request)
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

        PatchUserRequest patchUserRequest = new PatchUserRequest
        {
            Name = user.Name
        };

        request.ApplyTo(patchUserRequest);

        user.Name = patchUserRequest.Name;

        await _demoDbContext.SaveChangesAsync();

        return NoContent();
    }
}