using CleanArchitecture.Application.Products.Dtos;
using CleanArchitecture.Application.Products.Services;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace CleanArchitecture.UnitTests.Application;

public class ProductServiceTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_productRepository, _categoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_WithExistingCategory_PersistsProduct()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepository.ExistsAsync(categoryId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _service.CreateAsync(new CreateProductRequest("Mouse", "Wireless", 99.90m, categoryId));

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Mouse");
        await _productRepository.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_WithUnknownCategory_ReturnsValidationFailure()
    {
        _categoryRepository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var result = await _service.CreateAsync(new CreateProductRequest("Mouse", null, 99.90m, Guid.NewGuid()));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("validation");
        await _productRepository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        _productRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Product?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("not_found");
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_SoftDeletesIt()
    {
        var product = new Product("Mouse", null, 99.90m, Guid.NewGuid());
        _productRepository.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);

        var result = await _service.DeleteAsync(product.Id);

        result.IsSuccess.Should().BeTrue();
        product.Active.Should().BeFalse();
        _productRepository.Received(1).Update(product);
    }

    [Fact]
    public async Task SearchAsync_MapsPaginationMetadata()
    {
        var categoryId = Guid.NewGuid();
        var products = new List<Product>
        {
            new("Mouse", null, 99.90m, categoryId),
            new("Keyboard", null, 199.90m, categoryId)
        };

        _productRepository
            .SearchAsync(Arg.Any<ProductFilter>(), Arg.Any<CancellationToken>())
            .Returns(new PagedResult<Product>(products, 12, 2, 2));

        var result = await _service.SearchAsync(new ProductQuery { Page = 2, PageSize = 2 });

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalItems.Should().Be(12);
        result.Value.TotalPages.Should().Be(6);
        result.Value.HasPrevious.Should().BeTrue();
        result.Value.HasNext.Should().BeTrue();
    }
}
