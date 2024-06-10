using FluentValidation;
using Homework2.ProductService.Domain;

namespace Homework2.ProductService.Api.Validators.ControllersValidators;

public class AddProductValidator : AbstractValidator<ProductModelDto>
{
    public AddProductValidator()
    {
        RuleFor(r => r.Price).NotNull().GreaterThan(0);
        RuleFor(r => r.WarehouseId).NotNull().GreaterThan(0);
        RuleFor(r => r.Weight).NotNull().GreaterThan(0);
        RuleFor(r => r.Name).NotNull().MinimumLength(1);
        RuleFor(r => r.DateCreation).NotNull();
        RuleFor(r => r.Type).NotNull();
    }
}