using System.Net;
using System.Net.Http.Json;
using CleanArchitecture.Application.Categories.Dtos;
using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Products.Dtos;
using FluentAssertions;

namespace CleanArchitecture.IntegrationTests;

public class ProductsEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public ProductsEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_ThenGet_ReturnsCreatedProduct()
    {
        var categoryId = await CreateCategoryAsync();

        var createResponse = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("Integration Mouse", "Test product", 129.90m, categoryId));

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();
        var getResponse = await _client.GetAsync($"/api/products/{created!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<ProductResponse>();
        fetched!.Name.Should().Be("Integration Mouse");
        fetched.Price.Should().Be(129.90m);
    }

    [Fact]
    public async Task Post_WithUnknownCategory_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("Orphan Product", null, 10m, Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_SoftDeletesProduct()
    {
        var categoryId = await CreateCategoryAsync();

        var createResponse = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductRequest("Disposable Product", null, 49.90m, categoryId));

        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/products/{created!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetFromJsonAsync<ProductResponse>($"/api/products/{created.Id}");
        getResponse!.Active.Should().BeFalse();
    }

    [Fact]
    public async Task Get_WithPagination_RespectsPageSize()
    {
        var categoryId = await CreateCategoryAsync();

        for (var index = 0; index < 3; index++)
        {
            await _client.PostAsJsonAsync(
                "/api/products",
                new CreateProductRequest($"Paged Product {index}", null, 10m + index, categoryId));
        }

        var response = await _client.GetFromJsonAsync<PagedResponse<ProductResponse>>(
            $"/api/products?categoryId={categoryId}&pageSize=2&page=1");

        response!.Items.Should().HaveCount(2);
        response.TotalItems.Should().Be(3);
        response.TotalPages.Should().Be(2);
        response.HasNext.Should().BeTrue();
    }

    [Fact]
    public async Task Get_WithInvalidPageSize_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/products?pageSize=500");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<Guid> CreateCategoryAsync()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/categories",
            new CreateCategoryRequest($"Category {Guid.NewGuid()}", null));

        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>();

        return category!.Id;
    }
}
