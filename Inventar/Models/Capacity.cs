namespace Inventar.Models
{
    public class Capacity
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
