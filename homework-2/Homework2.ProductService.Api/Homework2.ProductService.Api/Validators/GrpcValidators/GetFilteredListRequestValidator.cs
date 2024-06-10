using FluentValidation;

namespace Homework2.ProductService.Api.Validators.GrpcValidators;

public class GetFilteredListRequestValidator : AbstractValidator<GetFilteredListRequest>
{
    public GetFilteredListRequestValidator()
    {
        RuleFor(r => r.WarehouseId).NotNull().GreaterThan(0);
        RuleFor(r => r.PageSize).NotNull().GreaterThan(0);
        RuleFor(r => r.Page).NotNull().GreaterThan(0);
        RuleFor(r => r.EndDate).NotNull();
        RuleFor(r => r.StartDate).NotNull();
        RuleFor(r => r.ProductType).NotNull();
    }
}