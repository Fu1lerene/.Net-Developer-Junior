using FluentValidation;

namespace Homework4.ProductService.Api.Validators.GrpcValidators;

public class GetFilteredListRequestValidator : AbstractValidator<GetFilteredListRequest>
{
    public GetFilteredListRequestValidator()
    {
        var positiveRule = "must be positive";
        var notNullRule = "can't be null";
        
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
            .WithMessage($"Number of Pages {positiveRule}");
        
        RuleFor(r => r.EndDate)
            .NotNull()
            .WithMessage($"EndDate {notNullRule}");
        
        RuleFor(r => r.StartDate)
            .NotNull()
            .WithMessage($"StartDate {notNullRule}");
        
        RuleFor(r => r.ProductType)
            .NotNull()
            .WithMessage($"ProductType {notNullRule}");
    }
}