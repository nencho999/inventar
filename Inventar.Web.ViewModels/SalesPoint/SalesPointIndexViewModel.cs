namespace Inventar.Web.ViewModels.SalesPoint;

public class SalesPointIndexViewModel
{
    public IEnumerable<SalesPointListDTO> SalesPoints { get; set; } = new List<SalesPointListDTO>();
    public string SearchTerm { get; set; }

    public int TotalCount { get; set; }
    public int PhysicalCount { get; set; }
    public int OnlineCount { get; set; }
    public int OperationalCount { get; set; }
    public int UnderConstructionCount { get; set; }
    public int ClosedCount { get; set; }

    public decimal TotalMonthlyExpenses { get; set; }
    public decimal TotalOneTimeExpenses { get; set; }
}