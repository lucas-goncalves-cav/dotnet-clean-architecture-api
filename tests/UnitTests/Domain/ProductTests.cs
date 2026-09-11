using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using FluentAssertions;

namespace CleanArchitecture.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesActiveProduct()
    {
        var categoryId = Guid.NewGuid();

        var product = new Product("Keyboard", "Mechanical keyboard", 199.90m, categoryId);

        product.Name.Should().Be("Keyboard");
        product.Price.Should().Be(199.90m);
        product.CategoryId.Should().Be(categoryId);
        product.Active.Should().BeTrue();
        product.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyName_Throws(string name)
    {
        var act = () => new Product(name, null, 10m, Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("Product name is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_WithNonPositivePrice_Throws(decimal price)
    {
        var act = () => new Product("Keyboard", null, price, Guid.NewGuid());

        act.Should().Throw<DomainException>().WithMessage("Product price must be greater than zero.");
    }

    [Fact]
    public void Update_ChangesValuesAndStampsUpdatedAt()
    {
        var product = new Product("Keyboard", null, 199.90m, Guid.NewGuid());
        var newCategoryId = Guid.NewGuid();

        product.Update("Mouse", "Wireless mouse", 99.90m, newCategoryId);

        product.Name.Should().Be("Mouse");
        product.Description.Should().Be("Wireless mouse");
        product.Price.Should().Be(99.90m);
        product.CategoryId.Should().Be(newCategoryId);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_MarksProductAsInactive()
    {
        var product = new Product("Keyboard", null, 199.90m, Guid.NewGuid());

        product.Deactivate();

        product.Active.Should().BeFalse();
        product.UpdatedAt.Should().NotBeNull();
    }
}
