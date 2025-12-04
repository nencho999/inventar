namespace Inventar.Models
{
    public class PrimaryMaterialBase
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<Capacity> Capacities { get; set; } = new List<Capacity>();
    }
}
