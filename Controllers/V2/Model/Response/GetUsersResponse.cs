using System;

namespace ProblemDetailsExample.Controllers.V2.Model.Response
{
    public class GetUsersResponse
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public bool IsDeleted { get; set; }
    }
}

