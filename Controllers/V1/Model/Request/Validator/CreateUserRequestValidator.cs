﻿using FluentValidation;
using FluentValidation.Results;
using ProblemDetailsExample.V1.Controllers.Model.Request;

namespace ProblemDetailsExample.Controllers.V1.Model.Request.Validator;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    protected override bool PreValidate(ValidationContext<CreateUserRequest> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure("Model", "Please ensure a model was supplied."));
            return false;
        }

        return true;
    }

    public CreateUserRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(model => model.Name).NotNull().NotEmpty();
        RuleFor(model => model.Surename).NotNull().NotEmpty();
    }
}