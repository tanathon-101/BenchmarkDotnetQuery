using Microsoft.EntityFrameworkCore;

namespace BenchmarkDotNetQuery
{
public class MyDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
}
}