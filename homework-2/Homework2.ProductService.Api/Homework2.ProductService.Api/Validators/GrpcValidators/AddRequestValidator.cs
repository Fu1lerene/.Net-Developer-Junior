using FluentValidation;

namespace Homework2.ProductService.Api.Validators.GrpcValidators;

public class AddRequestValidator : AbstractValidator<AddProductRequest>
{
    public AddRequestValidator()
    {
        RuleFor(r => r.Price).NotNull().GreaterThan(0);
        RuleFor(r => r.WarehouseId).NotNull().GreaterThan(0);
        RuleFor(r => r.Weight).NotNull().GreaterThan(0);
        RuleFor(r => r.Name).NotNull().MinimumLength(1);
        RuleFor(r => r.DateCreation).NotNull();
        RuleFor(r => r.ProductType).NotNull();
    }
}