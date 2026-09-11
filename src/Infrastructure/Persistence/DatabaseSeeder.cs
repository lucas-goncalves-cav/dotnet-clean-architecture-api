using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        if (await context.Categories.AnyAsync(cancellationToken))
        {
            return;
        }

        var electronics = new Category("Electronics", "Devices, gadgets and accessories");
        var books = new Category("Books", "Printed and digital books");
        var furniture = new Category("Furniture", "Home and office furniture");

        await context.Categories.AddRangeAsync([electronics, books, furniture], cancellationToken);

        await context.Products.AddRangeAsync(
        [
            new Product("Wireless Mouse", "Ergonomic wireless mouse", 149.90m, electronics.Id),
            new Product("Mechanical Keyboard", "Compact mechanical keyboard", 459.00m, electronics.Id),
            new Product("Noise Cancelling Headset", "Over ear headset", 899.90m, electronics.Id),
            new Product("Clean Architecture", "Robert C. Martin", 189.90m, books.Id),
            new Product("The Pragmatic Programmer", "Andrew Hunt and David Thomas", 209.90m, books.Id),
            new Product("Standing Desk", "Height adjustable desk", 1899.00m, furniture.Id),
            new Product("Office Chair", "Ergonomic office chair", 1299.00m, furniture.Id)
        ], cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
