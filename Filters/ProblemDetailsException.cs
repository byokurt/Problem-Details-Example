using Microsoft.AspNetCore.Mvc;

namespace ProblemDetailsExample.Filters;

public class ProblemDetailsException : Exception
{
    public ProblemDetailsException(ProblemDetails value)
    {
        Value = value;
    }

    public ProblemDetails Value { get; }
}