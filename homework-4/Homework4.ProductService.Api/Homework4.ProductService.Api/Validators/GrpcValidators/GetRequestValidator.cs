using FluentValidation;

namespace Homework4.ProductService.Api.Validators.GrpcValidators;

public class GetRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetRequestValidator()
    {
        RuleFor(r => r.ProductId)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("ProductID must be positive");
    }
}