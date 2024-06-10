using FluentValidation;
using Homework4.ProductService.Domain;

namespace Homework4.ProductService.Api.Validators.ControllersValidators;

public class GetFilteredListValidator : AbstractValidator<ProductFilter>
{
    public GetFilteredListValidator()
    {
        var positiveRule = "must be positive";
        var notNullRule = "cannot be null";

        RuleFor(r => r.WarehouseId)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"WarehouseID {positiveRule}");
        
        RuleFor(r => r.PageSize)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"PageSize {positiveRule}");
        
        RuleFor(r => r.Page)
            .NotNull()
            .GreaterThan(0)
            .WithMessage($"Number of pages {positiveRule}");
        
        RuleFor(r => r.EndDate)
            .NotNull()
            .WithMessage($"EndDate {notNullRule}");
        
        RuleFor(r => r.StartDate)
            .NotNull()
            .WithMessage($"StartDate {notNullRule}");
        
        RuleFor(r => r.Type)
            .NotNull()
            .WithMessage($"Type {notNullRule}");
    }
}