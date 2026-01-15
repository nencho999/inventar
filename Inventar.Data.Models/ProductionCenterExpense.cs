using System.ComponentModel.DataAnnotations;
public class ProductionCenterExpense
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public ExpenseType Type { get; set; }

    public DateTime? Date { get; set; }

    public Frequency? Frequency { get; set; }
    public int? EveryXMonths { get; set; }

    [Required]
    public Guid ProductionCenterId { get; set; }
    public ProductionCenter ProductionCenter { get; set; } = null!;
}

public enum ExpenseType { OneTime, Regular }
public enum Frequency { Weekly, Monthly, Yearly, CustomMonths }