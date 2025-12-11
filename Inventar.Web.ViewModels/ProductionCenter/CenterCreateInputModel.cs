using System.ComponentModel.DataAnnotations;

public class CenterCreateInputModel
{
    [Required]
    public string Name { get; set; }
    public string Location { get; set; }
    public string Capacity { get; set; }
    public string Contact { get; set; }
    public CenterStatus Status { get; set; }
    public string Expenses { get; set; }
}