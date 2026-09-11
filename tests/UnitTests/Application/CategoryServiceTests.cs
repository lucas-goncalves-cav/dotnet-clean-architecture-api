using CleanArchitecture.Application.Categories.Dtos;
using CleanArchitecture.Application.Categories.Services;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace CleanArchitecture.UnitTests.Application;

public class CategoryServiceTests
{
    private readonly ICategoryRepository _repository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _service = new CategoryService(_repository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_PersistsCategory()
    {
        var result = await _service.CreateAsync(new CreateCategoryRequest("Books", "Printed books"));

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Books");
        await _repository.Received(1).AddAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Category?)null);

        var result = await _service.UpdateAsync(Guid.NewGuid(), new UpdateCategoryRequest("Books", null));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("not_found");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedCategories()
    {
        _repository.GetAllAsync(true, Arg.Any<CancellationToken>())
            .Returns([new Category("Books", null), new Category("Electronics", null)]);

        var result = await _service.GetAllAsync(true);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}
