namespace Inventar.Web.ViewModels.Product
{
    public class ProductIndexViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ProductType { get; set; }
        public string? Package { get; set; }
        public decimal Price { get; set; }

        public decimal Vat { get; set; }

        public decimal Gain { get; set; }

        public int CentersCount { get; set; }
    }
}