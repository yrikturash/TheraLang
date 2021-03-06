﻿using System;
using System.Linq.Expressions;
using FluentValidation;
using TheraLang.Web.ViewModels;

namespace TheraLang.Web.Validators
{
    public class ProjectModelValidator : AbstractValidator<ProjectViewModel>
    {
        private static string ValidateStringLengthMessage =>
            "The field {PropertyName} has to be less than {MinLength} and more than {MaxLength}. Current length is {TotalLength}";

        public ProjectModelValidator()
        {
            ValidateStringLength(p => p.Name, 3, 55);
            RuleFor(p => p.TypeId).NotEqual(0)
                .WithMessage("Project type must be specified");
        }

        private void ValidateStringLength(Expression<Func<ProjectViewModel, string>> expression, int minLength,
            int maxLength)
        {
            RuleFor(expression)
                .NotEmpty()
                .Length(minLength, maxLength)
                .WithMessage(ValidateStringLengthMessage);
        }
    }
}