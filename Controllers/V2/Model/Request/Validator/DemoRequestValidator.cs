using FluentValidation;
using FluentValidation.Results;
using ProblemDetailsExample.V2.Controllers.Model.Request;

namespace ProblemDetailsExample.Controllers.V2.Model.Request.Validator;

public class DemoRequestValidator : AbstractValidator<DemoRequest>
{
    protected override bool PreValidate(ValidationContext<DemoRequest> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure("Model", "Please ensure a model was supplied."));
            return false;
        }

        return true;
    }

    public DemoRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.Title).NotNull().NotEmpty();
    }
}