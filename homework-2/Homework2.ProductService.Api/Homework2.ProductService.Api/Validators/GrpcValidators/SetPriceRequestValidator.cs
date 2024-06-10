using FluentValidation;

namespace Homework2.ProductService.Api.Validators.GrpcValidators;

public class SetPriceRequestValidator : AbstractValidator<SetPriceRequest>
{
    public SetPriceRequestValidator()
    {
        RuleFor(r => r.ProductId).GreaterThan(0);
        RuleFor(r => r.Price).GreaterThan(0);
    }
}