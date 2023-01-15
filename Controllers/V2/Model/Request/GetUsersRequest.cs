using System;
using ProblemDetailsExample.Models.Pagination;

namespace ProblemDetailsExample.Controllers.V2.Model.Request
{
    public class GetUsersRequest : PagedRequest
    {
        public int? Id { get; set; }

        public string? Title { get; set; }
    }
}

