using FluentValidation;
using Homework4.ProductService.Domain;

namespace Homework4.ProductService.Api.Validators.ControllersValidators;

public class SetPriceValidator : AbstractValidator<SetPriceModel>
{
    public SetPriceValidator()
    {
        var positiveRule = "must be positive";

        RuleFor(r => r.ProductId)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"ProductId {positiveRule}");

        RuleFor(r => r.Price)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"Price {positiveRule}");
    }
}