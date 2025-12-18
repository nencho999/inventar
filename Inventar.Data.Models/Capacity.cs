namespace Inventar.Data.Models
{
    public class Capacity
    {
        public Guid Id { get; set; }

        public Guid PrimaryMaterialBaseId { get; set; }
        public PrimaryMaterialBase PrimaryMaterialBase { get; set; } = null!;
        public Guid MaterialId { get; set; }
        public Material Material { get; set; } = null!;
        public decimal CapacityLimit { get; set; }
    }
}
