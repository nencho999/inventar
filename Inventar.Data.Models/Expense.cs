namespace Inventar.Data.Models;

public class Expense
{
    public Guid Id { get; set; }

    public Guid BaseId { get; set; }
    public PrimaryMaterialBase Base { get; set; }

    public string Name { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; } = null!;
    public bool IsCreatedByAdmin { get; set; }
}