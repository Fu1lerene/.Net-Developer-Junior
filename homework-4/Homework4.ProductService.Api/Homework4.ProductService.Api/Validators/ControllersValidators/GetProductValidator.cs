using FluentValidation;

namespace Homework4.ProductService.Api.Validators.ControllersValidators;

public class GetProductValidator : AbstractValidator<long>
{
    public GetProductValidator()
    {
        RuleFor(id => id)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("ProductId must be positive");
    }
}