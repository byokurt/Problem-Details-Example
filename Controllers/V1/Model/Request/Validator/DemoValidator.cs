using FluentValidation;
using FluentValidation.Results;
using ProblemDetailsExample.V1.Controllers.Model.Request;

namespace ProblemDetailsExample.Controllers.V1.Model.Request.Validator;

public class DemoValidator : AbstractValidator<Demo>
{
    protected override bool PreValidate(ValidationContext<Demo> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure("Model", "Please ensure a model was supplied."));
            return false;
        }

        return true;
    }

    public DemoValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.Title).NotNull().NotEmpty();
    }
}