using BenchmarkDotNetQuery.Model;

namespace BenchmarkDotNetQuery
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}