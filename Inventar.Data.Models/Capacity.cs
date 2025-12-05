namespace Inventar.Data.Models
{
    public class Capacity
    {
        public Guid Id { get; set; }

        public Guid PrimaryMaterialBaseId { get; set; }

        public PrimaryMaterialBase PrimaryMaterialBase { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
