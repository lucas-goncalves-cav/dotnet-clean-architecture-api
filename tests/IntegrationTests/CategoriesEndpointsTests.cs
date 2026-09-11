using System.Net;
using System.Net.Http.Json;
using CleanArchitecture.Application.Categories.Dtos;
using FluentAssertions;

namespace CleanArchitecture.IntegrationTests;

public class CategoriesEndpointsTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public CategoriesEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_WithValidPayload_CreatesCategory()
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest($"Tools {Guid.NewGuid()}", "Hand tools"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        created.Should().NotBeNull();
        created!.Active.Should().BeTrue();
    }

    [Fact]
    public async Task Post_WithEmptyName_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest(string.Empty, null));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Get_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/categories/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
