using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Homework4.ProductService.Api.Application;
using Homework4.ProductService.Api.Exceptions;
using Homework4.ProductService.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Homework4.ProductService.Api.Controllers;

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
    public IActionResult AddProduct(ProductModelDto productModel)
    {
        var validator = _serviceProvider.GetService<IValidator<ProductModelDto>>();
        var validationResult = validator?.Validate(productModel);

        if (validationResult is not null && !validationResult.IsValid)
            return BadRequest($"{string.Join(';', validationResult.Errors.Select(e => e.ErrorMessage))}");
        
        var productId = _productService.AddProduct(productModel);
        
        return Ok($"Продукт с {productId} ID успешно добавлен");
    }

    [HttpGet]
    public IEnumerable<ProductModel> GetAllProducts()
    {
        var products = _productService.GetAllProducts();
        return products;
    }

    [HttpGet]
    public IActionResult GetFilteredList([FromBody]ProductFilter productFilter)
    {
        var validator = _serviceProvider.GetService<IValidator<ProductFilter>>();
        var validationResult = validator?.Validate(productFilter);

        if (validationResult is not null && !validationResult.IsValid)
            return BadRequest($"{string.Join(';', validationResult.Errors.Select(e => e.ErrorMessage))}");
        
        var products = _productService.GetFilteredList(productFilter);
        return Ok(products);
    }

    [HttpPut]
    public IActionResult SetPrice([FromBody]SetPriceModel setPriceModel)
    {
        var validator = _serviceProvider.GetService<IValidator<SetPriceModel>>();
        var validationResult = validator?.Validate(new SetPriceModel
        {
            Price = setPriceModel.Price,
            ProductId = setPriceModel.ProductId
        });
        
        if (validationResult is not null && !validationResult.IsValid)
            return BadRequest($"{string.Join(';', validationResult.Errors.Select(e => e.ErrorMessage))}");
        
        try
        {
            _productService.SetPrice(setPriceModel);
            return Ok("Цена товара обновлена");
        }
        catch (ProductNotFoundException e)
        {
            Console.WriteLine(e.Message);
            return NotFound();
        }
    }

    [HttpGet("{productId:long}")]
    public IActionResult GetProduct(long productId)
    {
        var validator = _serviceProvider.GetService<IValidator<long>>();
        var validationResult = validator?.Validate(productId);

        if (validationResult is not null && !validationResult.IsValid)
            return BadRequest($"{string.Join(';', validationResult.Errors.Select(e => e.ErrorMessage))}");
        
        try
        {
            var product = _productService.GetProduct(productId);
            return Ok(product);
        }
        catch (ProductNotFoundException e)
        {
            Console.WriteLine(e.Message);
            return NotFound();
        }
    }
}