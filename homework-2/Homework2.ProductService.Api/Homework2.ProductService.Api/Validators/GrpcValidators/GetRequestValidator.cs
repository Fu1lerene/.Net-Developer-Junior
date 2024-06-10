using FluentValidation;

namespace Homework2.ProductService.Api.Validators.GrpcValidators;

public class GetRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetRequestValidator()
    {
        RuleFor(r => r.ProductId).GreaterThan(0);
    }
}