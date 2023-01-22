using FluentValidation;
using FluentValidation.Results;
using ProblemDetailsExample.V2.Controllers.Model.Request;

namespace ProblemDetailsExample.Controllers.V2.Model.Request.Validator;

public class QueryUserRequestValidator : AbstractValidator<QueryUsersRequest>
{
    protected override bool PreValidate(ValidationContext<QueryUsersRequest> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure("Model", "Please ensure a model was supplied."));
            return false;
        }

        return true;
    }

    public QueryUserRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.Page).GreaterThan(0);
        RuleFor(model => model.PageSize).ExclusiveBetween(0,100);
    }
}