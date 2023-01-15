using System;

namespace ProblemDetailsExample.Controllers.V1.Model.Response
{
    public class QueryUsersResponse
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public bool IsDeleted { get; set; }
    }
}

