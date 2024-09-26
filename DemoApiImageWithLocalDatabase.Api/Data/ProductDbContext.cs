using DemoApiImageWithLocalDatabase.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoApiImageWithLocalDatabase.Api.Data;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Product { get; init; } = default!;
}
