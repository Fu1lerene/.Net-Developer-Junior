using FluentValidation;
using Homework2.ProductService.Api.Application;
using Homework2.ProductService.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Homework2.ProductService.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IServiceProvider _serviceProvider;

    public ProductController(IProductService productService, IServiceProvider serviceProvider)
    {
        _productService = productService;
        _serviceProvider = serviceProvider;
    }
    
    [HttpPost]
    public string AddProduct(ProductModelDto productModel)
    {
        var validator = _serviceProvider.GetService<IValidator<ProductModelDto>>();
        var validationResult = validator?.Validate(productModel);

        if (validationResult is not null && !validationResult.IsValid)
            return $"Некорректный ввод: {string.Join(';', validationResult.Errors.Select(e => e.ErrorMessage))}";
        
        _productService.AddProduct(productModel);
        
        return $"Продукт с {_productService.GetAllProducts().Count()} ID успешно добавлен";
    }

    [HttpGet]
    public IEnumerable<ProductModel> GetAllProducts()
    {
        var products = _productService.GetAllProducts();
        return products;
    }

    [HttpGet]
    public IActionResult GetFilteredList([FromQuery]ProductFilter productFilter)
    {
        var validator = _serviceProvider.GetService<IValidator<ProductFilter>>();
        var validationResult = validator?.Validate(productFilter);

        if (validationResult is not null && !validationResult.IsValid)
            return Ok($"Некорректный ввод: {string.Join(';', validationResult.Errors.Select(e => e.ErrorMessage))}");
        
        var products = _productService.GetFilteredList(productFilter);
        return Ok(products);
    }

    [HttpPut]
    public IActionResult SetPrice(long productId, double newPrice)
    {
        if (productId < 1 || newPrice < 1)
        {
            return Ok("ID и price должны быть больше 0");
        }
        _productService.SetPrice(productId, newPrice);
        
        return Ok("Цена товара обновлена");
    }

    [HttpGet("{productId:long}")]
    public IActionResult GetProduct(long productId)
    {
        if (productId < 1)
        {
            return Ok("ID должен быть больше 0");
        }

        if (productId > _productService.GetAllProducts().Count())
        {
            return NotFound();
        }
        
        var product = _productService.GetProduct(productId);
        
        return Ok(product);
        
    }
}