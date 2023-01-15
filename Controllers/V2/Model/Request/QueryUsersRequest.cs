using System;
using ProblemDetailsExample.Models.Pagination;

namespace ProblemDetailsExample.Controllers.V2.Model.Request
{
    public class QueryUsersRequest : PagedRequest
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Surename { get; set; }
    }
}

