using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace BenchmarkDotNetQuery
{
    [MemoryDiagnoser]
    public class ProductBenchmark
    {
        private readonly string _connStr = "Server=localhost,1433;Database=BenmarkDb;User Id=SA;Password=p@ssw0rd;TrustServerCertificate=True;";
        private readonly MyDbContext _context;

        public ProductBenchmark()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlServer(_connStr)
                .Options;

            _context = new MyDbContext(options);
        }

        // 🔹 EF Core ToList
        [Benchmark]
        public List<Product> EFCore_ToList()
        {
            return _context.Products.ToList();
        }

        // 🔹 EF Core Include Supplier
        [Benchmark]
        public List<Product> EFCore_Include()
        {
            return _context.Products
                .Include(p => p.Supplier)
                .ToList();
        }

        // 🔹 EF Core Filter by Category
        [Benchmark]
        public List<Product> EFCore_Filter()
        {
            return _context.Products
                .Where(p => p.Category == "Electronics")
                .ToList();
        }

        // 🔹 Dapper Query All
        [Benchmark]
        public List<Product> Dapper_All()
        {
            using var conn = new SqlConnection(_connStr);
            return conn.Query<Product>("SELECT * FROM Products").ToList();
        }

        // 🔹 Dapper Join with Supplier
        [Benchmark]
        public List<dynamic> Dapper_Join()
        {
            using var conn = new SqlConnection(_connStr);
            string sql = @"
            SELECT p.*, s.Name AS SupplierName
            FROM Products p
            INNER JOIN Suppliers s ON p.SupplierId = s.Id";
            return conn.Query(sql).ToList();
        }

        // 🔹 Top 10 by Price
        [Benchmark]
        public List<Product> EFCore_Top10ByPrice()
        {
            return _context.Products
                .OrderByDescending(p => p.Price)
                .Take(10)
                .ToList();
        }
    }
}
