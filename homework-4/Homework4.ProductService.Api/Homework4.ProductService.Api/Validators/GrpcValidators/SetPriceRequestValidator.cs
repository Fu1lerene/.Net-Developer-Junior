using FluentValidation;

namespace Homework4.ProductService.Api.Validators.GrpcValidators;

public class SetPriceRequestValidator : AbstractValidator<SetPriceRequest>
{
    public SetPriceRequestValidator()
    {
        var positiveRule = "must be positive";
        
        RuleFor(r => r.ProductId)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"ProductID {positiveRule}");
        
        RuleFor(r => r.Price)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"Price {positiveRule}");
    }
}