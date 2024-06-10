using FluentValidation;

namespace Homework4.ProductService.Api.Validators.GrpcValidators;

public class AddRequestValidator : AbstractValidator<AddProductRequest>
{
    public AddRequestValidator()
    {
        var positiveRule = "must be positive";
        var notNullRule = "cannot be null";
        
        RuleFor(r => r.Price)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"Price {positiveRule}");
        
        RuleFor(r => r.WarehouseId)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"WarehouseId {positiveRule}");
        
        RuleFor(r => r.Weight)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"Weight {positiveRule}");
        
        RuleFor(r => r.Name)
            .NotNull()
            .NotEmpty()
            .MinimumLength(1)
            .WithMessage("Name must have at least one letter");
        
        RuleFor(r => r.DateCreation)
            .NotNull()
            .WithMessage($"DateCreation {notNullRule}");
        
        RuleFor(r => r.ProductType)
            .NotNull()
            .WithMessage($"ProductType {notNullRule}");
    }
}