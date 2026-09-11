using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using FluentAssertions;

namespace CleanArchitecture.UnitTests.Domain;

public class CategoryTests
{
    [Fact]
    public void Constructor_TrimsNameAndActivates()
    {
        var category = new Category("  Electronics  ", "Gadgets");

        category.Name.Should().Be("Electronics");
        category.Description.Should().Be("Gadgets");
        category.Active.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithEmptyName_Throws()
    {
        var act = () => new Category(" ", null);

        act.Should().Throw<DomainException>().WithMessage("Category name is required.");
    }

    [Fact]
    public void Reactivate_RestoresInactiveCategory()
    {
        var category = new Category("Books", null);
        category.Deactivate();

        category.Reactivate();

        category.Active.Should().BeTrue();
    }
}
