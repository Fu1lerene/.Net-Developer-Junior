using FluentValidation;
using Homework2.ProductService.Domain;

namespace Homework2.ProductService.Api.Validators.ControllersValidators;

public class GetFilteredListValidator : AbstractValidator<ProductFilter>
{
    public GetFilteredListValidator()
    {
        RuleFor(r => r.WarehouseId).NotNull().GreaterThan(0);
        RuleFor(r => r.PageSize).NotNull().GreaterThan(0);
        RuleFor(r => r.Page).NotNull().GreaterThan(0);
        RuleFor(r => r.EndDate).NotNull();
        RuleFor(r => r.StartDate).NotNull();
        RuleFor(r => r.Type).NotNull();
    }
}